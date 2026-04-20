using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Hayat.DAL.Entities;

namespace Hayat.DAL.Data
{
    public class HayatDbContext: DbContext
    {
        public HayatDbContext(DbContextOptions<HayatDbContext> options): base(options) { }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<ClinicSchedule> ClinicSchedules { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<VisitsHistory> MedicalHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all IEntityTypeConfiguration<T> classes from this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }


    }
