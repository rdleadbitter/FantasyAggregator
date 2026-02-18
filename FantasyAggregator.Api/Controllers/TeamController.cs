using Microsoft.AspNetCore.Mvc;
using FantasyAggregator.Business.Services;
using FantasyAggregatorApp.Models;

namespace FantasyAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly TeamService _service = new TeamService();

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var t = _service.Get(id);
            if (t == null) return NotFound();
            return Ok(t);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Team p)
        {
            var id = _service.Create(p);
            return CreatedAtAction(nameof(Get), new { id }, p);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Team t)
        {
            if (id != t.TeamId) return BadRequest("Id mismatch");
            var ok = _service.Update(t);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = _service.Delete(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
