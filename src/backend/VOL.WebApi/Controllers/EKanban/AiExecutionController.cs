using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EKanban.AiExecution;
using EKanban.IRepositories;
using EKanban.IServices;
using VOL.Entity.DomainModels;

namespace VOL.WebApi.Controllers.EKanban
{
    [Route("api/ekanban/[controller]/[action]")]
    public partial class AiExecutionController : ControllerBase
    {
        private readonly IAiExecutionService _aiExecutionService;
        private readonly IExecutionCardRepository _executionCardRepository;

        public AiExecutionController(
            IAiExecutionService aiExecutionService,
            IExecutionCardRepository executionCardRepository)
        {
            _aiExecutionService = aiExecutionService;
            _executionCardRepository = executionCardRepository;
        }

        [HttpPost]
        public async Task<IActionResult> TriggerExecution(int id)
        {
            var card = await _executionCardRepository.FindOneAsync(id);
            if (card == null)
            {
                return BadRequest(new { message = "Card not found" });
            }

            if (card.Status != (int)ExecutionCardStatus.Ready)
            {
                return BadRequest(new { message = "Card is not in Ready state" });
            }

            await _aiExecutionService.ExecuteAiTaskAsync(card);
            return Ok(new { message = "Execution started" });
        }
    }
}
