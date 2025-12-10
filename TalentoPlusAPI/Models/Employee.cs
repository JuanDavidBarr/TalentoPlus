using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Models
{
    public class Employee
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        
        [Required]
        [StringLength(20)]
        public string DocumentNumber { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }
        
        [StringLength(20)]
        public string Phone { get; set; }
        
        public DateTime BirthDate { get; set; }
        
        [StringLength(500)]
        public string Address { get; set; }
        
        [Required]
        public DateTime HireDate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; }
        
        public decimal Salary { get; set; }
        
        [StringLength(100)]
        public string EducationLevel { get; set; }
        
        [StringLength(1000)]
        public string ProfessionalProfile { get; set; }
        
        // Relationships
        public int PositionId { get; set; }
        public Position Position { get; set; }
        
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}