using FormBuilder.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Data
{
    public class FormBuilderAPIDbContext : DbContext
    {
        public FormBuilderAPIDbContext(DbContextOptions options) : base(options)
        {
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormGroup> FormGroups { get; set; }
        public DbSet<FormElement> FormElements { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Meta> Metas { get; set; }
        public DbSet<FormElementResult> FormElementResults { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fluent API configurations, if needed

            // Example: Configure the relationship between Form and User
            modelBuilder.Entity<Form>()
                .HasOne(f => f.User)
                .WithMany(u => u.Forms)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.SetNull); // Adjust as needed


            modelBuilder.Entity<Form>()
                .HasMany(f => f.FormGroups)
                .WithOne(fg => fg.Form)
                .HasForeignKey(fg => fg.FormId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Form>()
                .HasMany(f => f.FormElements)
                .WithOne(fe => fe.Form)
                .HasForeignKey(fe => fe.FormId)
                .OnDelete(DeleteBehavior.Cascade);

             modelBuilder.Entity<FormGroup>()
                .HasMany(fg => fg.FormElements)
                .WithOne(fe => fe.FormGroup)
                .HasForeignKey(fe => fe.GroupId)
                .OnDelete(DeleteBehavior.NoAction);

             modelBuilder.Entity<Answer>()
                .HasOne(a => a.User)
                .WithMany(u => u.Answers)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

             modelBuilder.Entity<Answer>()
                .HasOne(a => a.EvaluatedUser)
                .WithMany(u => u.EvaluationsReceived)
                .HasForeignKey(a => a.EvaluatedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                 .HasOne(a => a.FormElement)
                 .WithMany(fe => fe.Answers)
                 .HasForeignKey(a => a.FormElementId)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FormElementResult>()
                .HasOne(c => c.FormElement)
                .WithMany(fe => fe.FormElementResults) // Assuming you have a navigation property in FormElement
                .HasForeignKey(c => c.FormElementId)
                .OnDelete(DeleteBehavior.Restrict); // Adjust this based on your needs

            // Assuming you have a User entity
            modelBuilder.Entity<FormElementResult>()
                .HasOne(c => c.User)
                .WithMany(u => u.FormElementResults) // Assuming you have a navigation property in User
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Meta<int>>()
            //     .HasOne(m => m.RelatableEntity)
            //     .WithMany()
            //     .HasForeignKey(m => new { m.RelatableType, m.RelatableId })
            //     .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
