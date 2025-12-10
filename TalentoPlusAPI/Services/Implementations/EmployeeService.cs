using TalentoPlus.DTOs;
using TalentoPlus.Models;
using TalentoPlus.Repositories.Interfaces;
using TalentoPlus.Services.Interfaces;

namespace TalentoPlus.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _repository.GetAllAsync();
            return employees.Select(e => MapToDto(e));
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(int id)
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null)
            {
                return null;
            }
            return MapToDto(employee);
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createDto)
        {
            // Validate document number doesn't exist
            if (await _repository.DocumentNumberExistsAsync(createDto.DocumentNumber))
            {
                throw new InvalidOperationException("Document number already exists");
            }

            // Validate email doesn't exist
            if (await _repository.EmailExistsAsync(createDto.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            var employee = MapToEntity(createDto);
            var createdEmployee = await _repository.CreateAsync(employee);
            
            // Reload to get related entities
            var result = await _repository.GetByIdAsync(createdEmployee.Id);
            return MapToDto(result);
        }

        public async Task<EmployeeDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateDto)
        {
            var existingEmployee = await _repository.GetByIdAsync(id);
            if (existingEmployee == null)
            {
                return null;
            }

            // Validate document number doesn't exist for other employees
            if (await _repository.DocumentNumberExistsAsync(updateDto.DocumentNumber, id))
            {
                throw new InvalidOperationException("Document number already exists");
            }

            // Validate email doesn't exist for other employees
            if (await _repository.EmailExistsAsync(updateDto.Email, id))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Update properties
            existingEmployee.FirstName = updateDto.FirstName;
            existingEmployee.LastName = updateDto.LastName;
            existingEmployee.DocumentNumber = updateDto.DocumentNumber;
            existingEmployee.Email = updateDto.Email;
            existingEmployee.Phone = updateDto.Phone;
            existingEmployee.BirthDate = updateDto.BirthDate;
            existingEmployee.Address = updateDto.Address;
            existingEmployee.HireDate = updateDto.HireDate;
            existingEmployee.Status = updateDto.Status;
            existingEmployee.Salary = updateDto.Salary;
            existingEmployee.EducationLevel = updateDto.EducationLevel;
            existingEmployee.ProfessionalProfile = updateDto.ProfessionalProfile;
            existingEmployee.PositionId = updateDto.PositionId;
            existingEmployee.DepartmentId = updateDto.DepartmentId;

            var updatedEmployee = await _repository.UpdateAsync(existingEmployee);
            
            // Reload to get related entities
            var result = await _repository.GetByIdAsync(updatedEmployee.Id);
            return MapToDto(result);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        // Mapping methods
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

        private Employee MapToEntity(CreateEmployeeDto dto)
        {
            return new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DocumentNumber = dto.DocumentNumber,
                Email = dto.Email,
                Phone = dto.Phone,
                BirthDate = dto.BirthDate,
                Address = dto.Address,
                HireDate = dto.HireDate,
                Status = dto.Status,
                Salary = dto.Salary,
                EducationLevel = dto.EducationLevel,
                ProfessionalProfile = dto.ProfessionalProfile,
                PositionId = dto.PositionId,
                DepartmentId = dto.DepartmentId
            };
        }
    }
}