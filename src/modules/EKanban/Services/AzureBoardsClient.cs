using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using EKanban.Models;

namespace EKanban.Services;

/// <summary>
/// Azure Boards REST API 客户端
/// </summary>
public class AzureBoardsClient
{
    private readonly HttpClient _httpClient;
    private readonly string _organizationUrl;
    private readonly string _project;
    private readonly ILogger<AzureBoardsClient> _logger;

    public AzureBoardsClient(
        IConfiguration config,
        ILogger<AzureBoardsClient> logger)
    {
        _organizationUrl = config.GetValue<string>("AzureBoards:OrganizationUrl") ?? string.Empty;
        _project = config.GetValue<string>("AzureBoards:Project") ?? string.Empty;
        var pat = config.GetValue<string>("AzureBoards:PersonalAccessToken") ?? string.Empty;

        _httpClient = new HttpClient();
        var byteArray = System.Text.Encoding.ASCII.GetBytes($":{pat}");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        _logger = logger;
    }

    /// <summary>
    /// 获取所有工作项
    /// </summary>
    public async Task<List<AzureWorkItem>> GetAllWorkItemsAsync()
    {
        try
        {
            // WIQL 查询获取所有工作项 ID
            var wiql = new
            {
                query = @"SELECT [System.Id] FROM WorkItems WHERE [System.State] <> 'Removed' ORDER BY [System.ChangedDate] DESC"
            };

            var wiqlResponse = await _httpClient.PostAsJsonAsync(
                $"{_organizationUrl}/{_project}/_apis/wit/wiql?api-version=7.1-preview.2",
                wiql);

            wiqlResponse.EnsureSuccessStatusCode();
            var wiqlJson = await wiqlResponse.Content.ReadFromJsonAsync<WiqlResponse>();
            
            if (wiqlJson?.workItems == null || !wiqlJson.workItems.Any())
            {
                return new List<AzureWorkItem>();
            }

            var ids = string.Join(',', wiqlJson.workItems.Select(w => w.id));
            var workItemsResponse = await _httpClient.GetAsync(
                $"{_organizationUrl}/{_project}/_apis/wit/workitems?ids={ids}&$expand=all&api-version=7.1-preview.3");

            workItemsResponse.EnsureSuccessStatusCode();
            var workItemsJson = await workItemsResponse.Content.ReadFromJsonAsync<WorkItemsResponse>();

            var result = new List<AzureWorkItem>();
            foreach (var item in workItemsJson?.value ?? Enumerable.Empty<WorkItemJson>())
            {
                var workItem = new AzureWorkItem
                {
                    Id = item.id,
                    Title = GetField(item.fields, "System.Title"),
                    Description = GetField(item.fields, "System.Description"),
                    State = GetField(item.fields, "System.State"),
                    ChangedDate = GetDateTimeField(item.fields, "System.ChangedDate") ?? DateTime.UtcNow
                };
                result.Add(workItem);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get work items from Azure Boards");
            throw;
        }
    }

    /// <summary>
    /// 添加评论到工作项
    /// </summary>
    public async Task AddCommentAsync(int workItemId, string comment)
    {
        try
        {
            var url = $"{_organizationUrl}/{_project}/_apis/wit/workitems/{workItemId}/comments?api-version=7.1-preview.3";
            var content = new[]
            {
                new
                {
                    text = comment
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, content);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Added comment to work item {WorkItemId}", workItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add comment to work item {WorkItemId}", workItemId);
            throw;
        }
    }

    /// <summary>
    /// 更新工作项状态
    /// </summary>
    public async Task UpdateStateAsync(int workItemId, string state)
    {
        try
        {
            var url = $"{_organizationUrl}/{_project}/_apis/wit/workitems/{workItemId}?api-version=7.1-preview.3";
            var patchDocument = new[]
            {
                new
                {
                    op = "replace",
                    path = "/fields/System.State",
                    value = state
                }
            };

            var content = JsonContent.Create(patchDocument);
            var response = await _httpClient.PatchAsync(url, content);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Updated state to {State} for work item {WorkItemId}", state, workItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update state for work item {WorkItemId}", workItemId);
            throw;
        }
    }

    private string GetField(Dictionary<string, JsonElement> fields, string fieldName)
    {
        if (fields.TryGetValue(fieldName, out var value) && value.ValueKind == JsonValueKind.String)
        {
            return value.GetString() ?? string.Empty;
        }
        return string.Empty;
    }

    private DateTime? GetDateTimeField(Dictionary<string, JsonElement> fields, string fieldName)
    {
        if (fields.TryGetValue(fieldName, out var value))
        {
            if (value.ValueKind == JsonValueKind.String)
            {
                var str = value.GetString();
                if (DateTime.TryParse(str, out var date))
                {
                    return date;
                }
            }
        }
        return null;
    }
}

/// <summary>
/// Azure 工作项DTO
/// </summary>
public class AzureWorkItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string State { get; set; } = string.Empty;
    public DateTime ChangedDate { get; set; }
}

// JSON 响应模型
public class WiqlResponse
{
    public List<WiqlWorkItem> workItems { get; set; } = new();
}

public class WiqlWorkItem
{
    public int id { get; set; }
}

public class WorkItemsResponse
{
    public List<WorkItemJson> value { get; set; } = new();
}

public class WorkItemJson
{
    public int id { get; set; }
    public Dictionary<string, JsonElement> fields { get; set; } = new();
}
