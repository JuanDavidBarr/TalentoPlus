using Microsoft.AspNetCore.Mvc;
using TalentoPlus.DTOs;
using TalentoPlus.Services.Interfaces;

namespace TalentoPlus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeesController(IEmployeeService service)
        {
            _service = service;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllEmployees()
        {
            var employees = await _service.GetAllEmployeesAsync();
            return Ok(employees);
        }

        // GET: api/employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            var employee = await _service.GetEmployeeByIdAsync(id);
            
            if (employee == null)
            {
                return NotFound(new { message = $"Employee with ID {id} not found" });
            }

            return Ok(employee);
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var employee = await _service.CreateEmployeeAsync(createDto);
                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/employees/5
        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var employee = await _service.UpdateEmployeeAsync(id, updateDto);
                
                if (employee == null)
                {
                    return NotFound(new { message = $"Employee with ID {id} not found" });
                }

                return Ok(employee);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/employees/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            var result = await _service.DeleteEmployeeAsync(id);
            
            if (!result)
            {
                return NotFound(new { message = $"Employee with ID {id} not found" });
            }

            return NoContent();
        }
    }
}