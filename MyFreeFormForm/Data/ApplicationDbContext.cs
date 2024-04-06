using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFreeFormForm.Models; // Ensure you have the using directive for your models

namespace MyFreeFormForm.Data
{
    public class ApplicationDbContext : IdentityDbContext<MyIdentityUsers>
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
                .WithOne()
                .HasForeignKey(ff => ff.FormId);

            modelBuilder.Entity<Form>()
                .HasMany(f => f.FormNotes)
                .WithOne(fn => fn.Form)
                .HasForeignKey(fn => fn.FormId);
            base.OnModelCreating(modelBuilder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            modelBuilder.ApplyConfiguration(new IdentityUserConfiguration());
        }


        public class IdentityUserConfiguration : IEntityTypeConfiguration<MyIdentityUsers>
        {
            public void Configure(EntityTypeBuilder<MyIdentityUsers> builder)
            {
                builder.ToTable("AspNetUsers");
                builder.Property(p => p.FirstName).HasColumnName("FirstName");
                builder.Property(p => p.LastName).HasColumnName("LastName");
                builder.Property(p => p.City).HasColumnName("City");
                builder.Property(p => p.State).HasColumnName("State");
                builder.Property(p => p.Zip).HasColumnName("Zip");



            }
        }
    }
}
