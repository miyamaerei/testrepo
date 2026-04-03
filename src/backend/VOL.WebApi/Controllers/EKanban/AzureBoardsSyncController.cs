using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EKanban.IServices;

namespace VOL.WebApi.Controllers.EKanban
{
    [Route("api/ekanban/[controller]/[action]")]
    public partial class AzureBoardsSyncController : ControllerBase
    {
        private readonly ISyncService _syncService;

        public AzureBoardsSyncController(ISyncService syncService)
        {
            _syncService = syncService;
        }

        [HttpPost]
        public async Task<IActionResult> TriggerSync()
        {
            await _syncService.SyncFromAzureBoardsAsync();
            return Ok(new { message = "Sync completed" });
        }
    }
}
