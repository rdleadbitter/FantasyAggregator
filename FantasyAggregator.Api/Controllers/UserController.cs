using Microsoft.AspNetCore.Mvc;
using FantasyAggregator.Business.Services;
using FantasyAggregatorApp.Models;

namespace FantasyAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _service = new UserService();

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAllUsers());

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var p = _service.GetUser(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public IActionResult Create([FromBody] User u)
        {
            var id = _service.CreateUser(u);
            return CreatedAtAction(nameof(Get), new { id }, u);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] User u)
        {
            if (id != u.UserId) return BadRequest("Id mismatch");
            var ok = _service.UpdateUser(u);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = _service.DeleteUser(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
