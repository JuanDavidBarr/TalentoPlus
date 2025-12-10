using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El número de documento es requerido")]
        public string DocumentNumber { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }
    }
}