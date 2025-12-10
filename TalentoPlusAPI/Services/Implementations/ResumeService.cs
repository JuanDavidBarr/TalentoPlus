using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TalentoPlus.Models;
using TalentoPlus.Repositories.Interfaces;
using TalentoPlus.Services.Interfaces;

namespace TalentoPlus.Services.Implementations
{
    public class ResumeService : IResumeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public ResumeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<byte[]> GenerateEmployeeResumeAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found");
            }

            // Configure QuestPDF license (Community license is free)
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Element(c => ComposeHeader(c, employee));
                    page.Content().Element(c => ComposeContent(c, employee));
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container, Employee employee)
        {
            container.Column(column =>
            {
                column.Item().Background(Colors.Blue.Darken3).Padding(15).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text($"{employee.FirstName} {employee.LastName}")
                            .FontSize(24)
                            .Bold()
                            .FontColor(Colors.White);
                        
                        col.Item().Text(employee.Position?.Name ?? "Sin cargo asignado")
                            .FontSize(14)
                            .FontColor(Colors.White);
                    });
                });

                column.Item().PaddingVertical(5).LineHorizontal(2).LineColor(Colors.Blue.Darken3);
            });
        }

        private void ComposeContent(IContainer container, Employee employee)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Spacing(15);

                // Datos de Contacto
                column.Item().Element(c => ComposeSection(c, "Datos de Contacto", content =>
                {
                    content.Item().Text($"Email: {employee.Email}");
                    content.Item().Text($"Teléfono: {employee.Phone ?? "No especificado"}");
                    content.Item().Text($"Dirección: {employee.Address ?? "No especificada"}");
                }));

                // Datos Personales
                column.Item().Element(c => ComposeSection(c, "Datos Personales", content =>
                {
                    content.Item().Text($"Documento: {employee.DocumentNumber}");
                    content.Item().Text($"Fecha de Nacimiento: {employee.BirthDate:dd/MM/yyyy}");
                    content.Item().Text($"Edad: {CalculateAge(employee.BirthDate)} años");
                }));

                // Información Laboral
                column.Item().Element(c => ComposeSection(c, "Información Laboral", content =>
                {
                    content.Item().Text($"Cargo: {employee.Position?.Name ?? "No asignado"}");
                    content.Item().Text($"Departamento: {employee.Department?.Name ?? "No asignado"}");
                    content.Item().Text($"Fecha de Ingreso: {employee.HireDate:dd/MM/yyyy}");
                    content.Item().Text($"Antigüedad: {CalculateYearsOfService(employee.HireDate)}");
                    content.Item().Text($"Salario: {employee.Salary:C}");
                    content.Item().Text($"Estado: {employee.Status}");
                }));

                // Nivel Educativo
                column.Item().Element(c => ComposeSection(c, "Nivel Educativo", content =>
                {
                    content.Item().Text(employee.EducationLevel ?? "No especificado");
                }));

                // Perfil Profesional
                column.Item().Element(c => ComposeSection(c, "Perfil Profesional", content =>
                {
                    content.Item().Text(employee.ProfessionalProfile ?? "No especificado")
                        .LineHeight(1.4f);
                }));
            });
        }

        private void ComposeSection(IContainer container, string title, Action<ColumnDescriptor> contentAction)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingBottom(3)
                    .Text(title)
                    .FontSize(14)
                    .Bold()
                    .FontColor(Colors.Blue.Darken3);

                column.Item().PaddingTop(8).PaddingLeft(10).Column(contentAction);
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Column(column =>
            {
                column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                column.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(9)
                        .FontColor(Colors.Grey.Medium);
                    
                    row.RelativeItem().AlignRight().Text(text =>
                    {
                        text.Span("TalentoPlus - Sistema de Gestión de Talento Humano")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Medium);
                    });
                });
            });
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        private string CalculateYearsOfService(DateTime hireDate)
        {
            var today = DateTime.Today;
            var years = today.Year - hireDate.Year;
            var months = today.Month - hireDate.Month;
            
            if (months < 0)
            {
                years--;
                months += 12;
            }
            
            if (today.Day < hireDate.Day)
            {
                months--;
                if (months < 0)
                {
                    years--;
                    months += 12;
                }
            }

            if (years > 0 && months > 0)
                return $"{years} año(s) y {months} mes(es)";
            if (years > 0)
                return $"{years} año(s)";
            return $"{months} mes(es)";
        }
    }
}