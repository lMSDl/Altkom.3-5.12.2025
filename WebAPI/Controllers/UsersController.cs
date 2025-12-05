using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ICrudService<User> _service;
        public UsersController(ICrudService<User> service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            var user = await _service.ReadAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            var users = await _service.ReadAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] User user)
        {
            var userId = await _service.CreateAsync(user);
            return CreatedAtAction(nameof(Get), new { id = userId }, userId);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] User user)
        {
            var localUser = await _service.ReadAsync(id);
            if (localUser == null)
            {
                return NotFound();
            }
            var updated = await _service.UpdateAsync(user);
            if (!updated)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var localUser = await _service.ReadAsync(id);
            if (localUser == null)
            {
                return NotFound();
            }
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return BadRequest();
            }
            return NoContent();
        }
    }
}
