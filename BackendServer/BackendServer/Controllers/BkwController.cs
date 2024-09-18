using Microsoft.AspNetCore.Mvc;

namespace BackendServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BkwController : ControllerBase
    {
        // GET api/sample
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "Hello from REST API!" });
        }

        // GET api/sample/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(new { message = $"You requested data with ID {id}" });
        }

        // POST api/sample
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            return Ok(new { message = "Data received!", data = value });
        }

        // PUT api/sample/{id}
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            return Ok(new { message = $"Data with ID {id} updated!", newData = value });
        }

        // DELETE api/sample/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok(new { message = $"Data with ID {id} deleted!" });
        }
    }
}
