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

        private readonly IGenericRepository<ClinicSchedule> _clinicScheduleRepository;
        private readonly IGenericRepository<Doctor> _doctorRepository;

        public ReceptionistPortalService(
            IPatientRepository patientRepository,
            IAppointmentRepository appointmentRepository,
            IGenericRepository<Clinic> clinicRepository,
            IGenericRepository<Patient> patientReadRepository,
            IUnitOfWork unitOfWork,
            IGenericRepository<ClinicSchedule> clinicScheduleRepository,
            IGenericRepository<Doctor> doctorRepository)
        {
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _clinicRepository = clinicRepository;
            _patientReadRepository = patientReadRepository;
            _unitOfWork = unitOfWork;
            _clinicScheduleRepository = clinicScheduleRepository;
            _doctorRepository = doctorRepository;
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

        public async Task<RegisterPatientResponseDto> RegisterPatientAsync(RegisterPatientRequestDto request, CancellationToken cancellationToken = default)
        {
            var existingPatient = await _patientRepository.FirstOrDefaultAsync(
                p => p.NationalId == request.NationalId || p.Phone == request.Phone,
                cancellationToken);

            if (existingPatient != null)
            {
                throw new BusinessRuleException("A patient with the same National ID or Phone already exists.");
            }

            if (!Enum.TryParse<Gender>(request.Gender, true, out var parsedGender))
            {
                throw new BusinessRuleException("Invalid gender provided.");
            }

            var newPatient = new Patient
            {
                FullName = request.FullName,
                NationalId = request.NationalId,
                Gender = parsedGender,
                DateOfBirth = request.DateOfBirth,
                Phone = request.Phone,
                Address = request.Address
            };

            await _patientRepository.AddAsync(newPatient, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterPatientResponseDto
            {
                PatientId = newPatient.PatientId,
                FullName = newPatient.FullName,
                NationalId = newPatient.NationalId,
                Phone = newPatient.Phone
            };
        }

        public async Task<AppointmentSummaryDto> BookAppointmentAsync(Guid branchId, BookAppointmentRequestDto request, CancellationToken cancellationToken = default)
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

            var clinicDay = MapClinicDay(request.AppointmentDate.DayOfWeek);
            var schedule = await _clinicScheduleRepository.FirstOrDefaultAsync(
                s => s.ClinicId == clinic.ClinicId && s.DayOfWeek == clinicDay,
                cancellationToken);

            Guid? doctorId = schedule?.DoctorId;
            string doctorName = string.Empty;
            if (doctorId.HasValue)
            {
                var doctor = await _doctorRepository.GetByIdAsync(doctorId.Value, cancellationToken);
                doctorName = doctor?.FullName ?? string.Empty;
            }

            return MapAppointment(appointment, clinic.ClinicName, patient.FullName, doctorId, doctorName);
        }

        public async Task<IReadOnlyList<AppointmentSummaryDto>> GetAppointmentsForDateAsync(Guid branchId, DateOnly date, CancellationToken cancellationToken = default)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByBranchAndDateAsync(branchId, date, cancellationToken);

            return appointments
                .Select(appointment => 
                {
                    var appDay = MapClinicDay(appointment.AppointmentDate.DayOfWeek);
                    var appSchedule = appointment.Clinic?.ClinicSchedules?.FirstOrDefault(s => s.DayOfWeek == appDay);
                    var appDoctorId = appSchedule?.DoctorId;
                    var appDoctorName = appSchedule?.Doctor?.FullName ?? string.Empty;

                    return MapAppointment(
                        appointment,
                        appointment.Clinic?.ClinicName ?? string.Empty,
                        appointment.Patient?.FullName ?? string.Empty,
                        appDoctorId,
                        appDoctorName);
                })
                .ToList();
        }

        private static AppointmentSummaryDto MapAppointment(Appointment appointment, string clinicName, string patientName, Guid? doctorId, string doctorName)
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
                PatientName = patientName,
                DoctorId = doctorId,
                DoctorName = doctorName
            };
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
