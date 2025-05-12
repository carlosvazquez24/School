using Entities.Classes;
using Entities.Courses;
using Entities.Enrollments;
using Entities.Enum;
using Entities.Students;
using Entities.Teachers;
using Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }



        public DbSet<Student> Students { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<Class> Classes { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.HasPostgresEnum<EnumPeriod>();


            modelBuilder.Entity<Enrollment>().HasIndex(e => new { e.StudentId, e.ClassId }).IsUnique();

            //------------ 1 a muchos ---------------


            //Configuración desde el lado del "uno"(Course):

            modelBuilder.Entity<Course>()
                .HasMany(c=> c.Classes)  // Un CURSO tiene MUCHAS CLASES (HasMany)
                .WithOne(c => c.Course)     // Una CLASE pertenece a UN CURSO (WithOne)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // ¡Clases se borran automáticamente!


            // Ambas configuraciones son equivalentes, pero se escriben desde perspectivas diferentes.




       
        }


    }
}
