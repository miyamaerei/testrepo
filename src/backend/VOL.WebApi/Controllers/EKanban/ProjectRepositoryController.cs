using System.Collections.Generic;
using System.Threading.Tasks;
using EKanban.IServices;
using EKanban.Models;
using Microsoft.AspNetCore.Mvc;

namespace VOL.WebApi.Controllers.EKanban
{
    [Route("api/ekanban/[controller]/[action]")]
    public partial class ProjectRepositoryController : ControllerBase
    {
        private readonly IProjectRepositoriesService _service;

        public ProjectRepositoryController(IProjectRepositoriesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound(new { message = "Project repository not found" });
            }
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectRepositories entity)
        {
            var created = await _service.CreateAsync(entity);
            return Ok(created);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProjectRepositories entity)
        {
            var result = await _service.UpdateAsync(entity);
            if (!result)
            {
                return NotFound(new { message = "Project repository not found" });
            }
            return Ok(new { success = true });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Project repository not found" });
            }
            return Ok(new { success = true });
        }
    }
}
