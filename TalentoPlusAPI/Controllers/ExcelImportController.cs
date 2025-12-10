using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Services.Interfaces;

namespace TalentoPlus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExcelImportController : ControllerBase
    {
        private readonly IExcelImportService _excelImportService;
        private readonly IWebHostEnvironment _environment;

        public ExcelImportController(IExcelImportService excelImportService, IWebHostEnvironment environment)
        {
            _excelImportService = excelImportService;
            _environment = environment;
        }

        // POST: api/excelimport/upload
        [HttpPost("upload")]
        public async Task<ActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded" });
            }

            // Validate file extension
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                return BadRequest(new { message = "Only Excel files (.xlsx, .xls) are allowed" });
            }

            try
            {
                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Save file temporarily
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Import data
                var importedCount = await _excelImportService.ImportEmployeesFromExcelAsync(filePath);

                // Delete temporary file
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                return Ok(new 
                { 
                    message = "Import completed successfully",
                    importedEmployees = importedCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error importing file: {ex.Message}" });
            }
        }

        // POST: api/excelimport/import-from-path
        [HttpPost("import-from-path")]
        public async Task<ActionResult> ImportFromPath([FromBody] ImportPathRequest request)
        {
            if (string.IsNullOrEmpty(request.FilePath))
            {
                return BadRequest(new { message = "File path is required" });
            }

            try
            {
                var importedCount = await _excelImportService.ImportEmployeesFromExcelAsync(request.FilePath);

                return Ok(new 
                { 
                    message = "Import completed successfully",
                    importedEmployees = importedCount
                });
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error importing file: {ex.Message}" });
            }
        }
    }

    public class ImportPathRequest
    {
        public string FilePath { get; set; }
    }
}