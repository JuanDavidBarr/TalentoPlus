using System;
using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.DTOs
{
    public class UpdateEmployeeDto
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100)]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100)]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Document number is required")]
        [StringLength(20)]
        public string DocumentNumber { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(150)]
        public string Email { get; set; }
        
        [StringLength(20)]
        public string Phone { get; set; }
        
        [Required(ErrorMessage = "Birth date is required")]
        public DateTime BirthDate { get; set; }
        
        [StringLength(500)]
        public string Address { get; set; }
        
        [Required(ErrorMessage = "Hire date is required")]
        public DateTime HireDate { get; set; }
        
        [Required(ErrorMessage = "Status is required")]
        [StringLength(50)]
        public string Status { get; set; }
        
        [Required(ErrorMessage = "Salary is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be positive")]
        public decimal Salary { get; set; }
        
        [StringLength(100)]
        public string EducationLevel { get; set; }
        
        [StringLength(1000)]
        public string ProfessionalProfile { get; set; }
        
        [Required(ErrorMessage = "Position ID is required")]
        public int PositionId { get; set; }
        
        [Required(ErrorMessage = "Department ID is required")]
        public int DepartmentId { get; set; }
    }
}