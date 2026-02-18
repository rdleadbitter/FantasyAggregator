using Microsoft.AspNetCore.Mvc;
using FantasyAggregator.Business.Services;
using FantasyAggregatorApp.Models;

namespace FantasyAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformController : ControllerBase
    {
        private readonly PlatformService _service = new PlatformService();

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var p = _service.Get(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Platform p)
        {
            var id = _service.Create(p);
            return CreatedAtAction(nameof(Get), new { id }, p);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Platform p)
        {
            if (id != p.PlatformId) return BadRequest("Id mismatch");
            var ok = _service.Update(p);
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
