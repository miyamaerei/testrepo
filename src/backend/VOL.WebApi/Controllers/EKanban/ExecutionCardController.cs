using Microsoft.AspNetCore.Mvc;
using EKanban.IServices;
using EKanban.IRepositories;
using VOL.Entity.DomainModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VOL.WebApi.Controllers.EKanban
{
    [Route("api/ekanban/[controller]/[action]")]
    public partial class ExecutionCardController : ControllerBase
    {
        private readonly IExecutionCardService _service;
        private readonly IExecutionCardRepository _repository;

        public ExecutionCardController(
            IExecutionCardService service,
            IExecutionCardRepository repository)
        {
            _service = service;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _repository.FindOneAsync(id);
            if (data == null)
            {
                return NotFound(new { message = "Card not found" });
            }
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetKanbanData()
        {
            var allCards = await _repository.GetAllAsync();
            var grouped = allCards
                .GroupBy(c => c.Status)
                .ToDictionary(g => g.Key, g => g.ToList());
            return Ok(grouped);
        }

        [HttpPost]
        public async Task<IActionResult> TriggerReExecute(int id)
        {
            var card = await _repository.FindOneAsync(id);
            if (card == null)
            {
                return NotFound(new { message = "Card not found" });
            }

            // Reset needs manual intervention and transition back to Ready
            card.NeedsManualIntervention = false;
            card.FailureCount = 0;
            await _repository.UpdateAsync(card);

            // Transition to Ready (will be picked up by scheduler)
            // Actual transition will happen when scheduler runs
            return Ok(new { message = "Re-trigger scheduled" });
        }
    }
}
