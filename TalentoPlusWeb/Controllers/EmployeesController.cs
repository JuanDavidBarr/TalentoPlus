using Microsoft.AspNetCore.Mvc;
using TalentoPlusWeb.Models;
using TalentoPlusWeb.Services;

namespace TalentoPlusWeb.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeApiService _apiService;

        public EmployeesController(EmployeeApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _apiService.GetAllAsync();
            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _apiService.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            // Debug: mostrar errores de validación
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var e in error.Value.Errors)
                    {
                        Console.WriteLine($"Validation Error - {error.Key}: {e.ErrorMessage}");
                    }
                }
            }
            
            if (ModelState.IsValid)
            {
                var result = await _apiService.CreateAsync(employee);
                if (result != null)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Error al crear el empleado");
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _apiService.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var result = await _apiService.UpdateAsync(id, employee);
                if (result)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Error al actualizar el empleado");
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _apiService.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
