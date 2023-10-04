using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Student_Course_Enrollment_System.Models;

namespace Student_Course_Enrollment_System.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Student_Course_Enrollment_System.Models.Student> Students { get; set; } 

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<UserViewModel>? UserViewModel { get; set; }


        //Student-Enrollment: many-to-one relationship
        //Enrollement-Course: many-to-one relationship
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Enrollment>().HasKey(e =>e.EnrollmentID );

            modelBuilder.Entity<Enrollment>().HasOne(s => s.Student).WithMany(e => e.Enrollments).HasForeignKey(s => s.StudentID);

            modelBuilder.Entity<Enrollment>().HasOne(c => c.Course).WithMany(e => e.Enrollments).HasForeignKey(c => c.CourseID);

            base.OnModelCreating(modelBuilder);
        }


    }
}
