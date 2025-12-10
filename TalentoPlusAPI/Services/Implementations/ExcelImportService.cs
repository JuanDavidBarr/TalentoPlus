using OfficeOpenXml;
using TalentoPlus.Data;
using TalentoPlus.Models;
using TalentoPlus.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TalentoPlus.Services.Implementations
{
    public class ExcelImportService : IExcelImportService
    {
        private readonly PostgresqlContext _context;

        public ExcelImportService(PostgresqlContext context)
        {
            _context = context;
        }

        public async Task<int> ImportEmployeesFromExcelAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Excel file not found: {filePath}");
            }

            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            int importedCount = 0;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // First worksheet
                int rowCount = worksheet.Dimension.Rows;

                // Assuming first row is header
                // Column order: Documento, Nombres, Apellidos, FechaNacimiento, Direccion, Telefono, Email, Cargo, Salario, FechaIngreso, Estado, NivelEducativo, PerfilProfesional, Departamento
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        // Read data from Excel in the new order
                        var documentNumber = worksheet.Cells[row, 1].Text.Trim();  // Documento
                        var firstName = worksheet.Cells[row, 2].Text.Trim();       // Nombres
                        var lastName = worksheet.Cells[row, 3].Text.Trim();        // Apellidos
                        var birthDate = ParseDate(worksheet.Cells[row, 4].Text);   // FechaNacimiento
                        var address = worksheet.Cells[row, 5].Text.Trim();         // Direccion
                        var phone = worksheet.Cells[row, 6].Text.Trim();           // Telefono
                        var email = worksheet.Cells[row, 7].Text.Trim();           // Email
                        var positionName = worksheet.Cells[row, 8].Text.Trim();    // Cargo
                        var salary = ParseDecimal(worksheet.Cells[row, 9].Text);   // Salario
                        var hireDate = ParseDate(worksheet.Cells[row, 10].Text);   // FechaIngreso
                        var status = worksheet.Cells[row, 11].Text.Trim();         // Estado
                        var educationLevel = worksheet.Cells[row, 12].Text.Trim(); // NivelEducativo
                        var professionalProfile = worksheet.Cells[row, 13].Text.Trim(); // PerfilProfesional
                        var departmentName = worksheet.Cells[row, 14].Text.Trim(); // Departamento

                        // Skip if essential data is missing
                        if (string.IsNullOrEmpty(firstName) || 
                            string.IsNullOrEmpty(lastName) || 
                            string.IsNullOrEmpty(documentNumber))
                        {
                            Console.WriteLine($"Row {row}: Skipping - Missing essential data (Nombres, Apellidos, or Documento)");
                            continue;
                        }

                        // Check if employee already exists
                        var existingEmployee = await _context.Employees
                            .FirstOrDefaultAsync(e => e.DocumentNumber == documentNumber);

                        if (existingEmployee != null)
                        {
                            Console.WriteLine($"Row {row}: Skipping - Document number {documentNumber} already exists");
                            continue;
                        }

                        // Find or create Position
                        var position = await _context.Positions
                            .FirstOrDefaultAsync(p => p.Name == positionName);

                        if (position == null)
                        {
                            position = new Position
                            {
                                Name = positionName,
                                Description = $"Position imported from Excel"
                            };
                            _context.Positions.Add(position);
                            await _context.SaveChangesAsync();
                        }

                        // Find or create Department
                        var department = await _context.Departments
                            .FirstOrDefaultAsync(d => d.Name == departmentName);

                        if (department == null)
                        {
                            department = new Department
                            {
                                Name = departmentName,
                                Description = $"Department imported from Excel"
                            };
                            _context.Departments.Add(department);
                            await _context.SaveChangesAsync();
                        }

                        // Create employee
                        var employee = new Employee
                        {
                            DocumentNumber = documentNumber,
                            FirstName = firstName,
                            LastName = lastName,
                            BirthDate = birthDate,
                            Address = address,
                            Phone = phone,
                            Email = email,
                            Salary = salary,
                            HireDate = hireDate,
                            Status = string.IsNullOrEmpty(status) ? "Active" : status,
                            EducationLevel = educationLevel,
                            ProfessionalProfile = professionalProfile,
                            PositionId = position.Id,
                            DepartmentId = department.Id
                        };

                        _context.Employees.Add(employee);
                        await _context.SaveChangesAsync();
                        importedCount++;
                        
                        Console.WriteLine($"Row {row}: Successfully imported {firstName} {lastName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error importing row {row}: {ex.Message}");
                        continue;
                    }
                }
            }

            return importedCount;
        }

        private DateTime ParseDate(string dateText)
        {
            if (string.IsNullOrEmpty(dateText))
            {
                return DateTime.Now;
            }

            if (DateTime.TryParse(dateText, out DateTime result))
            {
                return result;
            }

            // Try parsing common date formats
            string[] formats = { 
                "dd/MM/yyyy", 
                "MM/dd/yyyy", 
                "yyyy-MM-dd",
                "dd-MM-yyyy",
                "MM-dd-yyyy"
            };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(dateText, format, null, System.Globalization.DateTimeStyles.None, out result))
                {
                    return result;
                }
            }

            return DateTime.Now;
        }

        private decimal ParseDecimal(string decimalText)
        {
            if (string.IsNullOrEmpty(decimalText))
            {
                return 0;
            }

            // Remove currency symbols and thousands separators
            decimalText = decimalText.Replace("$", "")
                                     .Replace("€", "")
                                     .Replace(",", "")
                                     .Replace(".", "")
                                     .Trim();

            if (decimal.TryParse(decimalText, out decimal result))
            {
                return result;
            }

            return 0;
        }
    }
}