using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.DTOs;
using TalentoPlus.DTOs.Auth;
using TalentoPlus.Services.Interfaces;

namespace TalentoPlus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IResumeService _resumeService;

        public AuthController(IAuthService authService, IResumeService resumeService)
        {
            _authService = authService;
            _resumeService = resumeService;
        }

        // ==================== ENDPOINTS PÚBLICOS ====================

        /// <summary>
        /// Lista todos los departamentos disponibles (público)
        /// </summary>
        [HttpGet("departments")]
        [AllowAnonymous]
        public async Task<ActionResult<List<DepartmentDto>>> GetDepartments()
        {
            var departments = await _authService.GetDepartmentsAsync();
            return Ok(departments);
        }

        /// <summary>
        /// Autoregistro de empleado (público)
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<EmployeeDto>> SelfRegister([FromBody] SelfRegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var employee = await _authService.SelfRegisterAsync(dto);
                return CreatedAtAction(nameof(GetMyInfo), employee);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error en el registro", details = ex.Message });
            }
        }

        /// <summary>
        /// Login de empleado (público)
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _authService.LoginAsync(dto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error en el login", details = ex.Message });
            }
        }

        // ==================== ENDPOINTS PROTEGIDOS ====================

        /// <summary>
        /// Obtener mi información (protegido con JWT)
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<EmployeeDto>> GetMyInfo()
        {
            try
            {
                var employeeId = GetEmployeeIdFromToken();
                var employee = await _authService.GetMyInfoAsync(employeeId);
                return Ok(employee);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener información", details = ex.Message });
            }
        }

        /// <summary>
        /// Descargar mi hoja de vida en PDF (protegido con JWT)
        /// </summary>
        [HttpGet("me/resume")]
        [Authorize]
        public async Task<IActionResult> GetMyResume()
        {
            try
            {
                var employeeId = GetEmployeeIdFromToken();
                var pdfBytes = await _resumeService.GenerateEmployeeResumeAsync(employeeId);
                return File(pdfBytes, "application/pdf", "MiHojaDeVida.pdf");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al generar PDF", details = ex.Message });
            }
        }

        private int GetEmployeeIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                              ?? User.FindFirst("sub");
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int employeeId))
            {
                throw new UnauthorizedAccessException("Token inválido");
            }

            return employeeId;
        }
    }
}