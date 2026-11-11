using EmployeeProject.Data;
using EmployeeProject.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace EmployeeProject
{
    public class EmployeeDbContext : IdentityDbContext<ApplicationIdentity>
    {
        public EmployeeDbContext(
            DbContextOptions<EmployeeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<EmployeeTask> EmployeeTasks { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<EmployeeLeaveDetail> EmployeeLeaveDetails { get; set; }

        public DbSet<PendingLeave> PendingLeaves { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Employee Primary Key
            modelBuilder.Entity<Employee>()
                .HasKey(e => e.EmpId);
            // Map Employee to the existing table
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Department>().ToTable("Departments");
            modelBuilder.Entity<Designation>().ToTable("Designations");
            modelBuilder.Entity<EmployeeLeaveDetail>().ToTable("EmployeeLeaveDetails");
            modelBuilder.Entity<PendingLeave>().ToTable("PendingLeaves");


            // Configure Salary precision
            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasPrecision(18, 2);

            //Gender
            modelBuilder.Entity<Employee>()
      .Property(e => e.Gender)
      .HasConversion<string>();

            //department
            modelBuilder.Entity<Employee>()
     .HasOne(e => e.Department)
     .WithMany(d => d.Employees)
     .HasForeignKey(e => e.DepartmentId);

            //Designation
            modelBuilder.Entity<Employee>()
    .HasOne(e => e.Designation)
    .WithMany(d => d.Employees)
    .HasForeignKey(e => e.DesignationId)
    .OnDelete(DeleteBehavior.Restrict);

            // Department - Designation
            modelBuilder.Entity<Designation>()
     .HasOne(d => d.Department)
     .WithMany(d => d.Designations)
     .HasForeignKey(d => d.DepartmentId)
     .OnDelete(DeleteBehavior.Restrict);

            // Employee - Reporting Person
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.ReportingPerson)
                .WithMany()
                .HasForeignKey(e => e.ReportingPersonId)
                .OnDelete(DeleteBehavior.Restrict);


            // EmployeeLeaveDetail - Primary Key
            modelBuilder.Entity<EmployeeLeaveDetail>()
                .HasKey(l => l.LeaveId);


            // EmployeeLeaveDetail - Employee
            modelBuilder.Entity<EmployeeLeaveDetail>()
                 .HasOne(l => l.Employee)
                .WithMany()
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);


            // EmployeeLeaveDetail - Reporting Person
            modelBuilder.Entity<EmployeeLeaveDetail>()
                .HasOne<Employee>()
                .WithMany()
                .HasForeignKey(l => l.ReportingPersonId)
                .OnDelete(DeleteBehavior.Restrict);


            // PendingLeave - Primary Key
            modelBuilder.Entity<PendingLeave>()
                .HasKey(p => p.PendingLeaveId);

            // Employee Primary Key
            modelBuilder.Entity<Employee>()
                .HasKey(e => e.EmpId);
            // PendingLeave - EmployeeLeaveDetail
            modelBuilder.Entity<PendingLeave>()
                .HasOne(p => p.EmployeeLeaveDetail)
                .WithOne()
                .HasForeignKey<PendingLeave>(p => p.LeaveId)
                .OnDelete(DeleteBehavior.Cascade);
            // PendingLeave - Reporting Person
            modelBuilder.Entity<PendingLeave>()
                .HasOne<Employee>()
                .WithMany()
                .HasForeignKey(p => p.ReportingPersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Timesheet>()
                .HasIndex(t => new { t.EmployeeId, t.WorkDate })
                .IsUnique();

            modelBuilder.Entity<Timesheet>()
                .HasOne(t => t.Employee)
                .WithMany(e => e.Timesheets)
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Timesheet>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Timesheets)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Timesheet>()
    .Property(t => t.Hours)
    .HasPrecision(5, 2);

        }
    }
}
