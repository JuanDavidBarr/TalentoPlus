using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Models
{
    public class Position
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public ICollection<Employee> Employees { get; set; }
    }
}

   