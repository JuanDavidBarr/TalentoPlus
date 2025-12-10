using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Services.Interfaces;

namespace TalentoPlus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumeController : ControllerBase
    {
        private readonly IResumeService _resumeService;

        public ResumeController(IResumeService resumeService)
        {
            _resumeService = resumeService;
        }

        // GET: api/resume/employee/5
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GenerateEmployeeResume(int employeeId)
        {
            try
            {
                var pdfBytes = await _resumeService.GenerateEmployeeResumeAsync(employeeId);
                
                return File(pdfBytes, "application/pdf", $"HojaDeVida_Empleado_{employeeId}.pdf");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating resume", details = ex.Message });
            }
        }
    }
}