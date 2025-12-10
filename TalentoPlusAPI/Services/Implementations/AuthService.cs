using Microsoft.EntityFrameworkCore;
using TalentoPlus.Data;
using TalentoPlus.DTOs;
using TalentoPlus.DTOs.Auth;
using TalentoPlus.Models;
using TalentoPlus.Services.Interfaces;

namespace TalentoPlus.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly PostgresqlContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;

        public AuthService(PostgresqlContext context, IJwtService jwtService, IEmailService emailService)
        {
            _context = context;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        public async Task<List<DepartmentDto>> GetDepartmentsAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description
                })
                .ToListAsync();
        }

        public async Task<EmployeeDto> SelfRegisterAsync(SelfRegisterDto dto)
        {
            // Check for existing employee with same document number or email
            var exists = await _context.Employees
                .AnyAsync(e => e.DocumentNumber == dto.DocumentNumber || e.Email == dto.Email);

            if (exists)
            {
                throw new InvalidOperationException("Ya existe un empleado con ese documento o correo electrónico.");
            }

            // Check if department exists
            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
            if (!departmentExists)
            {
                throw new InvalidOperationException("El departamento seleccionado no existe.");
            }

            // Create new employee
            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DocumentNumber = dto.DocumentNumber,
                Email = dto.Email,
                Phone = dto.Phone,
                BirthDate = dto.BirthDate,
                Address = dto.Address,
                HireDate = DateTime.UtcNow,
                Status = "Pendiente", // Estado inicial para autoregistros
                Salary = 0, // El salario lo asigna RRHH después
                EducationLevel = dto.EducationLevel,
                ProfessionalProfile = dto.ProfessionalProfile,
                PositionId = 4, // Posición por defecto (Administrative Assistant)
                DepartmentId = dto.DepartmentId
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Send welcome email
            await _emailService.SendWelcomeEmailAsync(employee.Email, $"{employee.FirstName} {employee.LastName}");

            // Load relationships for the DTO
            await _context.Entry(employee).Reference(e => e.Position).LoadAsync();
            await _context.Entry(employee).Reference(e => e.Department).LoadAsync();

            return MapToDto(employee);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.DocumentNumber == dto.DocumentNumber && e.Email == dto.Email);

            if (employee == null)
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            var token = _jwtService.GenerateToken(employee);
            var expirationHours = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_HOURS") ?? "24");

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(expirationHours),
                Employee = new EmployeeBasicInfo
                {
                    Id = employee.Id,
                    FullName = $"{employee.FirstName} {employee.LastName}",
                    Email = employee.Email
                }
            };
        }

        public async Task<EmployeeDto> GetMyInfoAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                throw new KeyNotFoundException("Empleado no encontrado.");
            }

            return MapToDto(employee);
        }

        private EmployeeDto MapToDto(Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DocumentNumber = employee.DocumentNumber,
                Email = employee.Email,
                Phone = employee.Phone,
                BirthDate = employee.BirthDate,
                Address = employee.Address,
                HireDate = employee.HireDate,
                Status = employee.Status,
                Salary = employee.Salary,
                EducationLevel = employee.EducationLevel,
                ProfessionalProfile = employee.ProfessionalProfile,
                PositionId = employee.PositionId,
                PositionName = employee.Position?.Name,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name
            };
        }
    }
}