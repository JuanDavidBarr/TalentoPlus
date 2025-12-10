using Microsoft.AspNetCore.Mvc;
using TalentoPlusWeb.Services;

namespace TalentoPlusWeb.Controllers
{
    public class ImportExportController : Controller
    {
        private readonly EmployeeApiService _apiService;

        public ImportExportController(EmployeeApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: ImportExport
        public IActionResult Index()
        {
            return View();
        }

        // POST: ImportExport/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["Error"] = "Por favor seleccione un archivo Excel.";
                return RedirectToAction(nameof(Index));
            }

            var extension = Path.GetExtension(excelFile.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                TempData["Error"] = "Solo se permiten archivos Excel (.xlsx, .xls).";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var stream = excelFile.OpenReadStream();
                var result = await _apiService.ImportExcelAsync(stream, excelFile.FileName);

                if (result.Success)
                {
                    TempData["Success"] = $"Importación exitosa. Se importaron {result.ImportedEmployees} empleados.";
                }
                else
                {
                    TempData["Error"] = $"Error en la importación: {result.Message}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ImportExport/DownloadResume/5
        public async Task<IActionResult> DownloadResume(int id)
        {
            try
            {
                var pdfBytes = await _apiService.GetEmployeeResumePdfAsync(id);

                if (pdfBytes == null)
                {
                    TempData["Error"] = "No se pudo generar la hoja de vida.";
                    return RedirectToAction("Index", "Employees");
                }

                return File(pdfBytes, "application/pdf", $"HojaDeVida_Empleado_{id}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al generar PDF: {ex.Message}";
                return RedirectToAction("Index", "Employees");
            }
        }
    }
}