using Microsoft.EntityFrameworkCore;
using TalentoPlus.Models;

namespace TalentoPlus.Data
{
    public class PostgresqlContext : DbContext
    {
        public PostgresqlContext(DbContextOptions<PostgresqlContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee configuration
            modelBuilder.Entity<Employee>(entity =>
            {
               
                entity.ToTable("employees");
                
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.DocumentNumber).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Phone);
                entity.Property(e => e.BirthDate).HasColumnType("date");
                entity.Property(e => e.Address);
                entity.Property(e => e.HireDate).HasColumnType("date").IsRequired();
                entity.Property(e => e.Status).HasDefaultValue("Active");
                entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.EducationLevel);
                entity.Property(e => e.ProfessionalProfile);
                
                // Relationships
                entity.HasOne(e => e.Position)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(e => e.PositionId);

                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Employees)
                    .HasForeignKey(e => e.DepartmentId);
            });

            // Position configuration
            modelBuilder.Entity<Position>(entity =>
            {
                entity.ToTable("positions");
                
                entity.Property(p => p.Name).IsRequired();
            });

            // Department configuration
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("departments");
                
                entity.Property(d => d.Name).IsRequired();
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Initial Positions
            modelBuilder.Entity<Position>().HasData(
                new Position { Id = 1, Name = "General Manager", Description = "Overall management responsibility" },
                new Position { Id = 2, Name = "Senior Developer", Description = "Software development and technical leadership" },
                new Position { Id = 3, Name = "HR Analyst", Description = "Human resources management and talent acquisition" },
                new Position { Id = 4, Name = "Administrative Assistant", Description = "Administrative support and office management" },
                new Position { Id = 5, Name = "Project Manager", Description = "Project planning and execution" },
                new Position { Id = 6, Name = "Data Analyst", Description = "Data analysis and reporting" }
            );

            // Initial Departments
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Management", Description = "Executive leadership and strategic planning" },
                new Department { Id = 2, Name = "Technology", Description = "Development, systems and IT infrastructure" },
                new Department { Id = 3, Name = "Human Resources", Description = "Talent management and organizational development" },
                new Department { Id = 4, Name = "Administration", Description = "Administrative operations and support" },
                new Department { Id = 5, Name = "Finance", Description = "Financial planning and accounting" },
                new Department { Id = 6, Name = "Marketing", Description = "Marketing and communications" }
            );
        }
    }
}