using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAPBD.Models;
using TestAPBD.Service;

namespace TestAPBD.Controller
{
    
    
    
    
    
    [Route("api/[controller]")]
    [ApiController]
    public class VisitsController : ControllerBase
    {
        private readonly IVisitsService _service;

        public VisitsController(IVisitsService service)
        {
            _service = service;
        }

        [HttpGet("{customerId}")]
        public async Task<OkObjectResult> getCustomerById(int customerId)
        {
            var result = await _service.getCustomerById(customerId);
            return Ok(result);
        }

        [HttpPost("{customerId}")]
        public async Task<IActionResult> addVisit([FromBody] VisitInsertDTO visit)
        {
            try
            {
                var result = _service.insertVisits(visit);
                return Ok(result);
            }
            catch (Exception e)
            {
                switch (e.Message)
                {
                    case "Visit already exists":
                        return BadRequest("Visit already exists");
                    case "Client does not exist":
                        return BadRequest("Client does not exist");
                    case "Mechanic does not exist":
                        return BadRequest("Mechanic does not exist");
                    case "Service does not exist":
                        return BadRequest("Service does not exist");
                }
            }
            
            return BadRequest();
        }
    }
}
