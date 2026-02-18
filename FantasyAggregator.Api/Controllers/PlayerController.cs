using Microsoft.AspNetCore.Mvc;
using FantasyAggregator.Business.Services;
using FantasyAggregatorApp.Models;

namespace FantasyAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly PlayerService _service = new PlayerService();

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAllPlayers());

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var p = _service.GetPlayerById(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [HttpGet("search/{term}")]
        public IActionResult Search(string term) => Ok(_service.SearchByName(term));

        [HttpPost]
        public IActionResult Create([FromBody] Player p)
        {
            var id = _service.CreatePlayer(p);
            return CreatedAtAction(nameof(Get), new { id }, p);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Player p)
        {
            if (id != p.PlayerId) return BadRequest("Id mismatch");
            var ok = _service.UpdatePlayer(p);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = _service.DeletePlayer(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
