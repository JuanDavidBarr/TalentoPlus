using System.ComponentModel.DataAnnotations;

namespace TalentoPlusWeb.Models
{
    public class Employee
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "El apellido es requerido")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "El documento es requerido")]
        [Display(Name = "Número de Documento")]
        public string DocumentNumber { get; set; }
        
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }
        
        [Display(Name = "Teléfono")]
        public string Phone { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime BirthDate { get; set; }
        
        [Display(Name = "Dirección")]
        public string Address { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Contratación")]
        public DateTime HireDate { get; set; }
        
        [Required]
        [Display(Name = "Estado")]
        public string Status { get; set; }
        
        [Required]
        [Display(Name = "Salario")]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }
        
        [Display(Name = "Nivel Educativo")]
        public string EducationLevel { get; set; }
        
        [Display(Name = "Perfil Profesional")]
        public string ProfessionalProfile { get; set; }
        
        [Display(Name = "Cargo")]
        public int PositionId { get; set; }
        
        [Display(Name = "Cargo")]
        public string PositionName { get; set; }
        
        [Display(Name = "Departamento")]
        public int DepartmentId { get; set; }
        
        [Display(Name = "Departamento")]
        public string DepartmentName { get; set; }
    }
}
