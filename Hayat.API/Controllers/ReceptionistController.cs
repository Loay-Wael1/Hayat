using Hayat.API.Infrastructure;
using Hayat.BLL.Constants;
using Hayat.BLL.DTOs.Receptionist;
using Hayat.BLL.DTOs.Shared;
using Hayat.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hayat.API.Controllers
{
    [ApiController]
    [Authorize(Roles = ApplicationRoles.Receptionist)]
    [Route("api/reception")]
    public class ReceptionistController : ControllerBase
    {
        private readonly IReceptionistPortalService _receptionistPortalService;

        public ReceptionistController(IReceptionistPortalService receptionistPortalService)
        {
            _receptionistPortalService = receptionistPortalService;
        }

        [HttpGet("patients/search")]
        public async Task<ActionResult<IReadOnlyList<PatientSearchResultDto>>> SearchPatients([FromQuery] string term, CancellationToken cancellationToken)
        {
            var results = await _receptionistPortalService.SearchPatientsAsync(term, cancellationToken);
            return Ok(results);
        }

        [HttpPost("patients")]
        public async Task<ActionResult<RegisterPatientResponseDto>> RegisterPatient([FromBody] RegisterPatientRequestDto request, CancellationToken cancellationToken)
        {
            var patient = await _receptionistPortalService.RegisterPatientAsync(request, cancellationToken);
            return Ok(patient);
        }

        [HttpPost("appointments/book")]
        public async Task<ActionResult<AppointmentSummaryDto>> Book([FromBody] BookAppointmentRequestDto request, CancellationToken cancellationToken)
        {
            if (!User.TryGetBranchId(out var branchId))
            {
                return Forbid();
            }

            var appointment = await _receptionistPortalService.BookAppointmentAsync(branchId, request, cancellationToken);
            return Ok(appointment);
        }

        [HttpGet("appointments/today")]
        public async Task<ActionResult<IReadOnlyList<AppointmentSummaryDto>>> GetTodayAppointments([FromQuery] DateOnly? date, CancellationToken cancellationToken)
        {
            if (!User.TryGetBranchId(out var branchId))
            {
                return Forbid();
            }

            var targetDate = date ?? DateOnly.FromDateTime(DateTime.Today);
            var appointments = await _receptionistPortalService.GetAppointmentsForDateAsync(branchId, targetDate, cancellationToken);
            return Ok(appointments);
        }
    }
}
