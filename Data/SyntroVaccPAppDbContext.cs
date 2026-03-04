using Microsoft.EntityFrameworkCore;
using SyntroVaccPApp.Models; 

namespace SyntroVaccPApp.Data
{
    public class SyntroVaccPAppDbContext : DbContext
    {
        public SyntroVaccPAppDbContext(DbContextOptions<SyntroVaccPAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Administration> Administrations { get; set; }     
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Vaccine> Vaccines { get; set; }
        public DbSet<VaccineBatch> VaccineBatches { get; set; }
        public DbSet<Clinician> Clinicians { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<PatientIdentifier> PatientIdentifiers { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<ExternalEntityMap> ExternalEntityMaps { get; set; }
        public DbSet<ExternalPatientMap> ExternalPatientMaps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             
            // ===============================
            // PatientIdentifiers
            // ===============================
            modelBuilder.Entity<PatientIdentifier>()
                .HasIndex(p => new { p.IdentifierType, p.IdentifierValue, p.CountryCode })
                .IsUnique();

            modelBuilder.Entity<PatientIdentifier>()
                .HasOne(pi => pi.Patient)
                .WithMany(p => p.PatientIdentifiers)
                .HasForeignKey(pi => pi.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PatientIdentifier>()
                .HasIndex(pi => new { pi.IdentifierValue, pi.IdentifierType, pi.CountryCode })
                .IsUnique()
                .HasDatabaseName("IX_PatientIdentifier_Unique");


            // ===============================
            // Administration
            // ===============================
            modelBuilder.Entity<Administration>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Administrations)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Administration>()
                .HasOne(a => a.Vaccine)
                .WithMany()
                .HasForeignKey(a => a.VaccineId)
                .OnDelete(DeleteBehavior.Restrict); // prevent deletion if admin exists

            modelBuilder.Entity<Administration>()
                .HasOne(a => a.Clinician)
                .WithMany()
                .HasForeignKey(a => a.ClinicianId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Administration>()
                .HasOne(a => a.Facility)
                .WithMany(f => f.Administrations)
                .HasForeignKey(a => a.FacilityId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Administration>()
                .HasOne(a => a.Batch)            //  navigation property
                .WithMany()
                .HasForeignKey(a => a.BatchId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Administration>()
                .HasIndex(a => new { a.PatientId, a.VaccineId, a.DoseNumber })
                .IsUnique()
                .HasDatabaseName("IX_Administration_UniqueDosePerPatient");


            // ===============================
            // Administration uniqueness constraint
            // Prevent same patient from having same vaccine + dose twice
            // ===============================
            modelBuilder.Entity<Administration>()
                .HasIndex(a => new { a.PatientId, a.VaccineId, a.DoseNumber })
                .IsUnique();

            modelBuilder.Entity<Administration>()
              .Property(a => a.CreatedUtc)
              .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Administration>()
                .Property(a => a.UpdatedUtc)
                .HasDefaultValueSql("NULL");

            // ===============================
            // VaccineBatch
            // ===============================
            modelBuilder.Entity<VaccineBatch>()
                .HasOne(vb => vb.Vaccine)
                .WithMany()
                .HasForeignKey(vb => vb.VaccineId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===============================
            // Patient
            // ===============================
            modelBuilder.Entity<Patient>()
                .Property(p => p.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Patient>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);
 

            // ===============================
            // AuditLog
            // ===============================
            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => new { a.EntityName, a.EntityId }); 
        }

    }
}
