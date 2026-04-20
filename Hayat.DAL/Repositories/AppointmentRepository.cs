using Hayat.DAL.Data;
using Hayat.DAL.Entities;
using Hayat.DAL.Entities.Enums;
using Hayat.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hayat.DAL.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(HayatDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Appointment>> GetAppointmentsByBranchAndDateAsync(Guid branchId, DateOnly date, CancellationToken cancellationToken = default)
        {
            var (start, end) = CreateDateRange(date);

            return await Context.Appointments
                .AsNoTracking()
                .Include(appointment => appointment.Patient)
                .Include(appointment => appointment.Clinic)
                .Where(appointment =>
                    appointment.AppointmentDate >= start &&
                    appointment.AppointmentDate < end &&
                    appointment.Clinic.BranchId == branchId)
                .OrderBy(appointment => appointment.AppointmentDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Appointment>> GetDoctorQueueAsync(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default)
        {
            var (start, end) = CreateDateRange(date);
            var clinicDay = MapClinicDay(date.DayOfWeek);

            var clinicIds = Context.ClinicSchedules
                .AsNoTracking()
                .Where(schedule => schedule.DoctorId == doctorId && schedule.DayOfWeek == clinicDay)
                .Select(schedule => schedule.ClinicId);

            return await Context.Appointments
                .AsNoTracking()
                .Include(appointment => appointment.Patient)
                .Include(appointment => appointment.Clinic)
                .Where(appointment =>
                    appointment.AppointmentDate >= start &&
                    appointment.AppointmentDate < end &&
                    clinicIds.Contains(appointment.ClinicId))
                .OrderBy(appointment => appointment.AppointmentDate)
                .ToListAsync(cancellationToken);
        }

        private static (DateTime Start, DateTime End) CreateDateRange(DateOnly date)
        {
            var start = date.ToDateTime(TimeOnly.MinValue);
            return (start, start.AddDays(1));
        }

        private static ClinicDayOfWeek MapClinicDay(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Saturday => ClinicDayOfWeek.Saturday,
                DayOfWeek.Sunday => ClinicDayOfWeek.Sunday,
                DayOfWeek.Monday => ClinicDayOfWeek.Monday,
                DayOfWeek.Tuesday => ClinicDayOfWeek.Tuesday,
                DayOfWeek.Wednesday => ClinicDayOfWeek.Wednesday,
                DayOfWeek.Thursday => ClinicDayOfWeek.Thursday,
                DayOfWeek.Friday => ClinicDayOfWeek.Friday,
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, "Unsupported day of week.")
            };
        }
    }
}
