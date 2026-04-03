using System.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EKanban.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VOL.Core.Extensions.AutofacManager;
using VOL.Entity.DomainModels;

namespace EKanban.Services
{
    public class AzureBoardsClient : IAzureBoardsClient, IDependency
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AzureBoardsClient> _logger;
        private readonly string _organization;
        private readonly string _project;
        private readonly string _personalAccessToken;

        public AzureBoardsClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AzureBoardsClient> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            _organization = _configuration["AzureBoards:Organization"];
            _project = _configuration["AzureBoards:Project"];
            _personalAccessToken = _configuration["AzureBoards:PersonalAccessToken"];

            var baseUrl = $"https://dev.azure.com/{_organization}/{_project}/_apis/wit/";
            _httpClient.BaseAddress = new Uri(baseUrl);

            var authToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{_personalAccessToken}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
        }

        public async Task<List<BoardWorkItem>> GetAllWorkItemsAsync()
        {
            var result = new List<BoardWorkItem>();
            try
            {
                // Wiql query to get all work items
                var query = new
                {
                    query = "SELECT [System.Id], [System.Title], [System.State], [System.Description] FROM WorkItems WHERE [System.State] <> 'Completed' AND [System.State] <> 'Removed'"
                };

                var response = await _httpClient.PostAsJsonAsync("wiql?api-version=7.0", query);
                response.EnsureSuccessStatusCode();

                var queryResult = await response.Content.ReadFromJsonAsync<WiqlQueryResult>();

                foreach (var itemRef in queryResult.workItems)
                {
                    var workItem = await GetWorkItemByIdAsync(itemRef.id);
                    if (workItem != null)
                    {
                        result.Add(workItem);
                    }
                }

                _logger.LogInformation($"Successfully fetched {result.Count} work items from Azure Boards");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get work items from Azure Boards");
            }

            return result;
        }

        public async Task<BoardWorkItem> GetWorkItemByIdAsync(int azureWorkItemId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"workitems/{azureWorkItemId}?api-version=7.0&$expand=fields");
                response.EnsureSuccessStatusCode();

                var item = await response.Content.ReadFromJsonAsync<AzureWorkItem>();

                var boardWorkItem = new BoardWorkItem
                {
                    AzureWorkItemId = azureWorkItemId,
                    Title = item.fields["System.Title"]?.ToString() ?? string.Empty,
                    Description = item.fields.ContainsKey("System.Description") ? item.fields["System.Description"]?.ToString() : null,
                    AzureState = item.fields["System.State"]?.ToString() ?? string.Empty,
                    Url = $"https://dev.azure.com/{_organization}/{_project}/_workitems/edit/{azureWorkItemId}",
                    CreatedDate = DateTime.UtcNow,
                    LastSyncDate = DateTime.UtcNow
                };

                return boardWorkItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get work item {azureWorkItemId} from Azure Boards");
                return null;
            }
        }

        public async Task AddCommentAsync(int azureWorkItemId, string comment)
        {
            try
            {
                var commentRequest = new
                {
                    text = comment
                };

                await _httpClient.PostAsJsonAsync($"workitems/{azureWorkItemId}/comments?api-version=7.1-preview.1", commentRequest);
                _logger.LogInformation($"Added comment to work item {azureWorkItemId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add comment to work item {azureWorkItemId}");
            }
        }

        public async Task UpdateStateAsync(int azureWorkItemId, string state)
        {
            try
            {
                var patchDocument = new List<object>
                {
                    new
                    {
                        op = "replace",
                        path = "/fields/System.State",
                        value = state
                    }
                };

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"workitems/{azureWorkItemId}?api-version=7.0")
                {
                    Content = JsonContent.Create(patchDocument)
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                _logger.LogInformation($"Updated state to {state} for work item {azureWorkItemId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update state for work item {azureWorkItemId}");
            }
        }

        // Response classes for Azure API
        private class WiqlQueryResult
        {
            public List<WorkItemRef> workItems { get; set; }
        }

        private class WorkItemRef
        {
            public int id { get; set; }
        }

        private class AzureWorkItem
        {
            public Dictionary<string, object> fields { get; set; } = new Dictionary<string, object>();
        }
    }
}
