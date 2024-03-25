using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFreeFormForm.Models; // Ensure you have the using directive for your models

namespace MyFreeFormForm.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add DbSets for your application models
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormField> FormFields { get; set; }
        public DbSet<FormNotes> FormNotes { get; set; }
        public DbSet<FormSubmissionQueue> FormSubmissionQueue { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // If you have any model-specific configurations, do them here
            // For example, configuring relationships or constraints
            modelBuilder.Entity<Form>()
                .HasMany(f => f.FormFields)
                .WithOne(ff => ff.Form)
                .HasForeignKey(ff => ff.FormId);

            modelBuilder.Entity<Form>()
                .HasMany(f => f.FormNotes)
                .WithOne(fn => fn.Form)
                .HasForeignKey(fn => fn.FormId);
        }
    }
}
