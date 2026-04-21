using Hayat.BLL.DTOs.Doctor;
using Hayat.BLL.Interfaces;
using Hayat.DAL.Entities.Enums;
using Hayat.DAL.Interfaces;

namespace Hayat.BLL.Services
{
    public class DoctorPortalService : IDoctorPortalService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IVisitsHistoryRepository _visitsHistoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DoctorPortalService(
            IAppointmentRepository appointmentRepository,
            IVisitsHistoryRepository visitsHistoryRepository,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _visitsHistoryRepository = visitsHistoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<DoctorQueueItemDto>> GetQueueAsync(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default)
        {
            var appointments = await _appointmentRepository.GetDoctorQueueAsync(doctorId, date, cancellationToken);

            return appointments
                .Where(appointment =>
                    appointment.Status != AppointmentStatus.Cancelled &&
                    appointment.Status != AppointmentStatus.Completed)
                .Select(appointment => new DoctorQueueItemDto
                {
                    AppointmentId = appointment.AppointmentId,
                    AppointmentDate = appointment.AppointmentDate,
                    Status = appointment.Status.ToString(),
                    PatientId = appointment.PatientId,
                    PatientName = appointment.Patient.FullName,
                    NationalId = appointment.Patient.NationalId,
                    Phone = appointment.Patient.Phone,
                    ClinicId = appointment.ClinicId,
                    ClinicName = appointment.Clinic.ClinicName
                })
                .ToList();
        }

        public async Task<IReadOnlyList<MedicalHistoryTimelineItemDto>> GetMedicalHistoryTimelineAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            var timeline = await _visitsHistoryRepository.GetPatientTimelineAsync(patientId, cancellationToken);

            return timeline
                .Select(history => new MedicalHistoryTimelineItemDto
                {
                    VisitId = history.Id,
                    CreatedAt = history.CreatedAt,
                    DoctorId = history.DoctorId,
                    DoctorName = history.Doctor.FullName,
                    Complaint = history.PatientComplaint,
                    Diagnosis = history.Diagnosis,
                    Notes = history.Notes
                })
                .ToList();
        }

        public async Task<UpdateAppointmentStatusResponseDto> UpdateAppointmentStatusAsync(int appointmentId, UpdateAppointmentStatusRequestDto request, CancellationToken cancellationToken = default)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);

            if (appointment == null)
            {
                throw new Exceptions.EntityNotFoundException("Appointment not found.");
            }

            if (request.Status == AppointmentStatus.Scheduled)
            {
                throw new Exceptions.BusinessRuleException("Doctor cannot move appointment status back to Scheduled.");
            }

            appointment.Status = request.Status;

            _appointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateAppointmentStatusResponseDto
            {
                AppointmentId = appointment.AppointmentId,
                Status = appointment.Status,
                Updated = true
            };
        }
    }
}
