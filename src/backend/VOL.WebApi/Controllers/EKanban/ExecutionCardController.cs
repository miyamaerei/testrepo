using Microsoft.AspNetCore.Mvc;
using EKanban.IServices;
using EKanban.IRepositories;
using VOL.Entity.DomainModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VOL.Core.Extensions;

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
            var data = await _repository.FindFirstAsync(X=>X.Id==id);
            if (data == null)
            {
                return NotFound(new { message = "Card not found" });
            }
            return Ok(data);
        }

        /// <summary>
        /// 获取卡片详情（包含阶段进度和文件变更）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDetail(int id)
        {
            try
            {
                var detail = await _service.GetDetailAsync(id);
                return Ok(VOL.Core.Utilities.WebResponseContent.Instance.OK("获取卡片详情成功", detail));
            }
            catch (System.ArgumentException ex)
            {
                return Ok(VOL.Core.Utilities.WebResponseContent.Instance.Error(ex.Message));
            }
            catch (System.Exception ex)
            {
                return Ok(VOL.Core.Utilities.WebResponseContent.Instance.Error("获取卡片详情失败: " + ex.Message));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetKanbanData()
        {
            var allCards = await _repository.FindAsync(X=>true);
            var grouped = allCards
                .GroupBy(c => c.Status)
                .ToDictionary(g => g.Key, g => g.ToList());
            return Ok(grouped);
        }

        [HttpPost]
        public async Task<IActionResult> TriggerReExecute(int id)
        {
            var card = await _repository.FindFirstAsync(X => X.Id == id);
            if (card == null)
            {
                return NotFound(new { message = "Card not found" });
            }

            // Reset needs manual intervention and transition back to Ready
            card.NeedsManualIntervention = false;
            card.FailureCount = 0;
            _repository.Update(card);

            // Transition to Ready (will be picked up by scheduler)
            // Actual transition will happen when scheduler runs
            return Ok(new { message = "Re-trigger scheduled" });
        }

        /// <summary>
        /// 手动创建看板卡片
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateManualCard([FromBody] ManualCardCreateRequest request)
        {
            try
            {
                var card = await _service.CreateManualCardAsync(request);
                return Ok(card);
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "创建失败: " + ex.Message });
            }
        }

        /// <summary>
        /// 编辑手动看板卡片
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateManualCard([FromBody] ManualCardUpdateRequest request)
        {
            try
            {
                var card = await _service.UpdateManualCardAsync(request);
                return Ok(card);
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "更新失败: " + ex.Message });
            }
        }

        /// <summary>
        /// 删除手动看板卡片
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteManualCard(int id)
        {
            try
            {
                await _service.DeleteManualCardAsync(id);
                return Ok(new { message = "删除成功" });
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "删除失败: " + ex.Message });
            }
        }

        /// <summary>
        /// 获取所有手动创建的看板卡片（支持分页和筛选）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetManualCards(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? status = null)
        {
            try
            {
                var (items, total) = await _service.GetManualCardsAsync(page, pageSize, search, status);
                return Ok(new { total, items });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "获取卡片列表失败: " + ex.Message });
            }
        }
    }
}
