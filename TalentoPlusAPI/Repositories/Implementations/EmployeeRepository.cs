using Microsoft.EntityFrameworkCore;
using TalentoPlus.Data;
using TalentoPlus.Models;
using TalentoPlus.Repositories.Interfaces;

namespace TalentoPlus.Repositories.Implementations
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly PostgresqlContext _context;

        public EmployeeRepository(PostgresqlContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee> CreateAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return false;
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> DocumentNumberExistsAsync(string documentNumber, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Employees
                    .AnyAsync(e => e.DocumentNumber == documentNumber && e.Id != excludeId.Value);
            }
            return await _context.Employees.AnyAsync(e => e.DocumentNumber == documentNumber);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Employees
                    .AnyAsync(e => e.Email == email && e.Id != excludeId.Value);
            }
            return await _context.Employees.AnyAsync(e => e.Email == email);
        }
    }
}