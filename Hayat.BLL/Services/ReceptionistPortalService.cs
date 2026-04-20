using Hayat.BLL.DTOs.Receptionist;
using Hayat.BLL.DTOs.Shared;
using Hayat.BLL.Exceptions;
using Hayat.BLL.Interfaces;
using Hayat.DAL.Entities;
using Hayat.DAL.Entities.Enums;
using Hayat.DAL.Interfaces;

namespace Hayat.BLL.Services
{
    public class ReceptionistPortalService : IReceptionistPortalService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IGenericRepository<Clinic> _clinicRepository;
        private readonly IGenericRepository<Patient> _patientReadRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReceptionistPortalService(
            IPatientRepository patientRepository,
            IAppointmentRepository appointmentRepository,
            IGenericRepository<Clinic> clinicRepository,
            IGenericRepository<Patient> patientReadRepository,
            IUnitOfWork unitOfWork)
        {
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _clinicRepository = clinicRepository;
            _patientReadRepository = patientReadRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<PatientSearchResultDto>> SearchPatientsAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var patients = await _patientRepository.SearchAsync(searchTerm, cancellationToken: cancellationToken);

            return patients
                .Select(patient => new PatientSearchResultDto
                {
                    PatientId = patient.PatientId,
                    FullName = patient.FullName,
                    NationalId = patient.NationalId,
                    Phone = patient.Phone,
                    Gender = patient.Gender.ToString(),
                    DateOfBirth = patient.DateOfBirth
                })
                .ToList();
        }

        public async Task<AppointmentSummaryDto> QuickBookAsync(Guid branchId, QuickBookAppointmentRequestDto request, CancellationToken cancellationToken = default)
        {
            if (request.AppointmentDate < DateTime.Today)
            {
                throw new BusinessRuleException("Appointments cannot be booked in the past.");
            }

            var clinic = await _clinicRepository.FirstOrDefaultAsync(
                existingClinic => existingClinic.ClinicId == request.ClinicId && existingClinic.BranchId == branchId,
                cancellationToken);

            if (clinic is null)
            {
                throw new EntityNotFoundException("Clinic was not found for the current branch.");
            }

            var patient = await _patientReadRepository.GetByIdAsync(request.PatientId, cancellationToken);
            if (patient is null)
            {
                throw new EntityNotFoundException("Patient was not found.");
            }

            var alreadyBooked = await _appointmentRepository.AnyAsync(
                appointment =>
                    appointment.ClinicId == request.ClinicId &&
                    appointment.PatientId == request.PatientId &&
                    appointment.AppointmentDate == request.AppointmentDate &&
                    appointment.Status != AppointmentStatus.Cancelled,
                cancellationToken);

            if (alreadyBooked)
            {
                throw new BusinessRuleException("An appointment already exists for the same patient, clinic, and time.");
            }

            var appointment = new Appointment
            {
                AppointmentDate = request.AppointmentDate,
                CreatedAt = DateTime.Now,
                ClinicId = clinic.ClinicId,
                PatientId = patient.PatientId,
                Status = AppointmentStatus.Scheduled
            };

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapAppointment(appointment, clinic.ClinicName, patient.FullName);
        }

        public async Task<IReadOnlyList<AppointmentSummaryDto>> GetAppointmentsForDateAsync(Guid branchId, DateOnly date, CancellationToken cancellationToken = default)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByBranchAndDateAsync(branchId, date, cancellationToken);

            return appointments
                .Select(appointment => MapAppointment(
                    appointment,
                    appointment.Clinic.ClinicName,
                    appointment.Patient.FullName))
                .ToList();
        }

        private static AppointmentSummaryDto MapAppointment(Appointment appointment, string clinicName, string patientName)
        {
            return new AppointmentSummaryDto
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                CreatedAt = appointment.CreatedAt,
                Status = appointment.Status.ToString(),
                ClinicId = appointment.ClinicId,
                ClinicName = clinicName,
                PatientId = appointment.PatientId,
                PatientName = patientName
            };
        }
    }
}
