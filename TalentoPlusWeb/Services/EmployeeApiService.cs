using System.Text;
using System.Text.Json;
using TalentoPlusWeb.Models;

namespace TalentoPlusWeb.Services
{
    public class EmployeeApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public EmployeeApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // ==================== EMPLOYEES ====================

        public async Task<List<Employee>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("api/employees");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Employee>>(json, _jsonOptions) ?? new List<Employee>();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/employees/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Employee>(json, _jsonOptions);
        }

        public async Task<Employee?> CreateAsync(Employee employee)
        {
            var json = JsonSerializer.Serialize(employee, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/employees", content);
            if (!response.IsSuccessStatusCode) return null;
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Employee>(responseJson, _jsonOptions);
        }

        public async Task<bool> UpdateAsync(int id, Employee employee)
        {
            var json = JsonSerializer.Serialize(employee, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/employees/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/employees/{id}");
            return response.IsSuccessStatusCode;
        }

        // ==================== EXCEL IMPORT ====================

        public async Task<ImportResult> ImportExcelAsync(Stream fileStream, string fileName)
        {
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            content.Add(streamContent, "file", fileName);

            var response = await _httpClient.PostAsync("api/excelimport/upload", content);
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ImportResult>(json, _jsonOptions);
                return result ?? new ImportResult { Success = true, Message = "Importación completada" };
            }

            return new ImportResult { Success = false, Message = json };
        }

        // ==================== PDF RESUME ====================

        public async Task<byte[]?> GetEmployeeResumePdfAsync(int employeeId)
        {
            var response = await _httpClient.GetAsync($"api/resume/employee/{employeeId}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsByteArrayAsync();
        }

        // ==================== AUTH - PUBLIC ====================

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            var response = await _httpClient.GetAsync("api/auth/departments");
            if (!response.IsSuccessStatusCode) return new List<Department>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Department>>(json, _jsonOptions) ?? new List<Department>();
        }

        public async Task<RegisterResult> SelfRegisterAsync(SelfRegisterModel model)
        {
            var json = JsonSerializer.Serialize(model, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/auth/register", content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new RegisterResult { Success = true, Message = "Registro exitoso. Revisa tu correo electrónico." };
            }

            try
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(responseJson, _jsonOptions);
                return new RegisterResult { Success = false, Message = error?.Message ?? "Error en el registro" };
            }
            catch
            {
                return new RegisterResult { Success = false, Message = "Error en el registro" };
            }
        }

        public async Task<LoginResult> LoginAsync(LoginModel model)
        {
            var json = JsonSerializer.Serialize(model, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/auth/login", content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseJson, _jsonOptions);
                return new LoginResult 
                { 
                    Success = true, 
                    Token = loginResponse?.Token,
                    EmployeeName = loginResponse?.Employee?.FullName,
                    EmployeeEmail = loginResponse?.Employee?.Email
                };
            }

            try
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(responseJson, _jsonOptions);
                return new LoginResult { Success = false, Message = error?.Message ?? "Credenciales inválidas" };
            }
            catch
            {
                return new LoginResult { Success = false, Message = "Error en el login" };
            }
        }

        // ==================== AUTH - PROTECTED ====================

        public async Task<Employee?> GetMyInfoAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/me");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Employee>(json, _jsonOptions);
        }

        public async Task<byte[]?> GetMyResumePdfAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/me/resume");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;
            
            return await response.Content.ReadAsByteArrayAsync();
        }
    }

    // ==================== MODELS ====================

    public class ImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ImportedEmployees { get; set; }
    }

    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class SelfRegisterModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Address { get; set; }
        public string? EducationLevel { get; set; }
        public string? ProfessionalProfile { get; set; }
        public int DepartmentId { get; set; }
    }

    public class RegisterResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class LoginModel
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeEmail { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public EmployeeBasicInfo? Employee { get; set; }
    }

    public class EmployeeBasicInfo
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}