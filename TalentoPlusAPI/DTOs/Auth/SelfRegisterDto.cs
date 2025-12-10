using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.DTOs.Auth
{
    public class SelfRegisterDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "El número de documento es requerido")]
        [StringLength(20)]
        public string DocumentNumber { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(150)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime BirthDate { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(100)]
        public string EducationLevel { get; set; }

        [StringLength(1000)]
        public string ProfessionalProfile { get; set; }

        [Required(ErrorMessage = "El departamento es requerido")]
        public int DepartmentId { get; set; }
    }
}