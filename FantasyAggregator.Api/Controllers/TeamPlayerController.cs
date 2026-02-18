using Microsoft.AspNetCore.Mvc;
using FantasyAggregator.Business.Services;
using FantasyAggregatorApp.Models;

namespace FantasyAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamPlayersController : ControllerBase
    {
        private readonly TeamPlayerService _svc = new TeamPlayerService();

        [HttpGet]
        public IActionResult GetAll() => Ok(_svc.GetAll());

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var t = _svc.Get(id);
            if (t == null) return NotFound();
            return Ok(t);
        }

        [HttpPost]
        public IActionResult Create([FromBody] TeamPlayer tp)
        {
            try
            {
                var id = _svc.Create(tp);
                return CreatedAtAction(nameof(Get), new { id }, tp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] TeamPlayer tp)
        {
            if (id != tp.TeamPlayerId) return BadRequest("Id mismatch");
            var ok = _svc.Update(tp);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = _svc.Delete(id);
            return ok ? NoContent() : NotFound();
        }

        // GET api/TeamPlayers/roster/1
        [HttpGet("roster/{teamId:int}")]
        public IActionResult Roster(int teamId)
        {
            var roster = _svc.GetRosterWithNames(teamId);
            return Ok(roster.Select(t => new {
                t.Item1.TeamPlayerId,
                t.Item1.TeamId,
                t.Item1.PlayerId,
                PlayerName = t.Item2,
                t.Item1.RosterSlot,
                t.Item1.AcquiredOn
            }));
        }
    }
}
