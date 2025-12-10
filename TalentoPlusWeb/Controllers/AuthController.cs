using Microsoft.AspNetCore.Mvc;
using TalentoPlusWeb.Services;

namespace TalentoPlusWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly EmployeeApiService _apiService;

        public AuthController(EmployeeApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            // Si ya está logueado, redirigir al portal
            if (HttpContext.Session.GetString("Token") != null)
            {
                return RedirectToAction(nameof(Portal));
            }
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _apiService.LoginAsync(model);

            if (result.Success && !string.IsNullOrEmpty(result.Token))
            {
                // Guardar token en sesión
                HttpContext.Session.SetString("Token", result.Token);
                HttpContext.Session.SetString("EmployeeName", result.EmployeeName ?? "");
                HttpContext.Session.SetString("EmployeeEmail", result.EmployeeEmail ?? "");

                TempData["Success"] = $"¡Bienvenido, {result.EmployeeName}!";
                return RedirectToAction(nameof(Portal));
            }

            ModelState.AddModelError("", result.Message ?? "Credenciales inválidas");
            return View(model);
        }

        // GET: Auth/Register
        public async Task<IActionResult> Register()
        {
            var departments = await _apiService.GetDepartmentsAsync();
            ViewBag.Departments = departments;
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(SelfRegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _apiService.GetDepartmentsAsync();
                ViewBag.Departments = departments;
                return View(model);
            }

            var result = await _apiService.SelfRegisterAsync(model);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(Login));
            }

            ModelState.AddModelError("", result.Message);
            var depts = await _apiService.GetDepartmentsAsync();
            ViewBag.Departments = depts;
            return View(model);
        }

        // GET: Auth/Portal (Protegido - requiere sesión)
        public async Task<IActionResult> Portal()
        {
            var token = HttpContext.Session.GetString("Token");
            
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Debes iniciar sesión para acceder al portal.";
                return RedirectToAction(nameof(Login));
            }

            var employee = await _apiService.GetMyInfoAsync(token);
            
            if (employee == null)
            {
                HttpContext.Session.Clear();
                TempData["Error"] = "Sesión expirada. Por favor inicia sesión nuevamente.";
                return RedirectToAction(nameof(Login));
            }

            return View(employee);
        }

        // GET: Auth/DownloadMyResume
        public async Task<IActionResult> DownloadMyResume()
        {
            var token = HttpContext.Session.GetString("Token");
            
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Debes iniciar sesión para descargar tu hoja de vida.";
                return RedirectToAction(nameof(Login));
            }

            var pdfBytes = await _apiService.GetMyResumePdfAsync(token);
            
            if (pdfBytes == null)
            {
                TempData["Error"] = "No se pudo generar tu hoja de vida.";
                return RedirectToAction(nameof(Portal));
            }

            return File(pdfBytes, "application/pdf", "MiHojaDeVida.pdf");
        }

        // POST: Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Has cerrado sesión correctamente.";
            return RedirectToAction(nameof(Login));
        }
    }
}