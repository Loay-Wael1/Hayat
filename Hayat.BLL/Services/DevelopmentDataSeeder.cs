using Hayat.BLL.Constants;
using Hayat.BLL.Interfaces;
using Hayat.DAL.Entities;
using Hayat.DAL.Entities.Enums;
using Hayat.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Hayat.BLL.Services
{
    public class DevelopmentDataSeeder : IDevelopmentDataSeeder
    {
        private const string BranchName = "AlQudah Main Branch";
        private const string ClinicName = "General Medicine";
        private const string DoctorName = "Dr. Amal Qudah";
        private const string DoctorUserName = "doctor.alqudah";
        private const string DoctorEmail = "doctor@alqudah.local";
        private const string DoctorPassword = "Doctor@123";
        private const string ReceptionistDisplayName = "Mona Reception";
        private const string ReceptionistUserName = "reception.alqudah";
        private const string ReceptionistEmail = "reception@alqudah.local";
        private const string ReceptionistPassword = "Reception@123";

        private readonly IGenericRepository<Branch> _branchRepository;
        private readonly IGenericRepository<Clinic> _clinicRepository;
        private readonly IGenericRepository<Doctor> _doctorRepository;
        private readonly IGenericRepository<ClinicSchedule> _clinicScheduleRepository;
        private readonly IGenericRepository<Patient> _patientRepository;
        private readonly IGenericRepository<VisitsHistory> _visitsHistoryRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public DevelopmentDataSeeder(
            IGenericRepository<Branch> branchRepository,
            IGenericRepository<Clinic> clinicRepository,
            IGenericRepository<Doctor> doctorRepository,
            IGenericRepository<ClinicSchedule> clinicScheduleRepository,
            IGenericRepository<Patient> patientRepository,
            IGenericRepository<VisitsHistory> visitsHistoryRepository,
            IAppointmentRepository appointmentRepository,
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            _branchRepository = branchRepository;
            _clinicRepository = clinicRepository;
            _doctorRepository = doctorRepository;
            _clinicScheduleRepository = clinicScheduleRepository;
            _patientRepository = patientRepository;
            _visitsHistoryRepository = visitsHistoryRepository;
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            var branch = await EnsureBranchAsync(cancellationToken);
            var clinic = await EnsureClinicAsync(branch.BranchId, cancellationToken);
            var doctor = await EnsureDoctorAsync(cancellationToken);
            var patients = await EnsurePatientsAsync(cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await EnsureSchedulesAsync(clinic.ClinicId, doctor.DoctorId, cancellationToken);
            await EnsureAppointmentsAsync(clinic.ClinicId, patients, cancellationToken);
            await EnsureMedicalHistoryAsync(doctor.DoctorId, patients, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await EnsureRolesAsync();
            await EnsureDoctorUserAsync(doctor.DoctorId);
            await EnsureReceptionistUserAsync(branch.BranchId);
        }

        private async Task<Branch> EnsureBranchAsync(CancellationToken cancellationToken)
        {
            var branch = await _branchRepository.FirstOrDefaultAsync(
                existingBranch => existingBranch.BranchName == BranchName,
                cancellationToken);

            if (branch is not null)
            {
                return branch;
            }

            branch = new Branch
            {
                BranchName = BranchName,
                Phone = "0790000000",
                City = "Amman"
            };

            await _branchRepository.AddAsync(branch, cancellationToken);
            return branch;
        }

        private async Task<Clinic> EnsureClinicAsync(Guid branchId, CancellationToken cancellationToken)
        {
            var clinic = await _clinicRepository.FirstOrDefaultAsync(
                existingClinic => existingClinic.ClinicName == ClinicName && existingClinic.BranchId == branchId,
                cancellationToken);

            if (clinic is not null)
            {
                return clinic;
            }

            clinic = new Clinic
            {
                ClinicName = ClinicName,
                BranchId = branchId
            };

            await _clinicRepository.AddAsync(clinic, cancellationToken);
            return clinic;
        }

        private async Task<Doctor> EnsureDoctorAsync(CancellationToken cancellationToken)
        {
            var doctor = await _doctorRepository.FirstOrDefaultAsync(
                existingDoctor => existingDoctor.FullName == DoctorName,
                cancellationToken);

            if (doctor is not null)
            {
                return doctor;
            }

            doctor = new Doctor
            {
                FullName = DoctorName,
                Specialty = "Internal Medicine"
            };

            await _doctorRepository.AddAsync(doctor, cancellationToken);
            return doctor;
        }

        private async Task<List<Patient>> EnsurePatientsAsync(CancellationToken cancellationToken)
        {
            return
            [
                await EnsurePatientAsync("Rania Saleh", "29901011234567", Gender.Female, new DateTime(1999, 1, 1), "0791111111", "Amman", cancellationToken),
                await EnsurePatientAsync("Omar Nasser", "19807021234567", Gender.Male, new DateTime(1980, 7, 2), "0792222222", "Zarqa", cancellationToken)
            ];
        }

        private async Task<Patient> EnsurePatientAsync(
            string fullName,
            string nationalId,
            Gender gender,
            DateTime dateOfBirth,
            string phone,
            string address,
            CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.FirstOrDefaultAsync(
                existingPatient => existingPatient.NationalId == nationalId,
                cancellationToken);

            if (patient is not null)
            {
                return patient;
            }

            patient = new Patient
            {
                FullName = fullName,
                NationalId = nationalId,
                Gender = gender,
                DateOfBirth = dateOfBirth,
                Phone = phone,
                Address = address
            };

            await _patientRepository.AddAsync(patient, cancellationToken);
            return patient;
        }

        private async Task EnsureSchedulesAsync(int clinicId, Guid doctorId, CancellationToken cancellationToken)
        {
            var existingSchedules = await _clinicScheduleRepository.CountAsync(
                schedule => schedule.ClinicId == clinicId && schedule.DoctorId == doctorId,
                cancellationToken);

            if (existingSchedules > 0)
            {
                return;
            }

            var schedules = Enum.GetValues<ClinicDayOfWeek>()
                .Select(day => new ClinicSchedule
                {
                    ClinicId = clinicId,
                    DoctorId = doctorId,
                    DayOfWeek = day,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0)
                });

            await _clinicScheduleRepository.AddRangeAsync(schedules, cancellationToken);
        }

        private async Task EnsureAppointmentsAsync(int clinicId, IReadOnlyList<Patient> patients, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;

            await EnsureAppointmentAsync(clinicId, patients[0].PatientId, today.AddHours(10), AppointmentStatus.Waiting, cancellationToken);
            await EnsureAppointmentAsync(clinicId, patients[1].PatientId, today.AddHours(10.5), AppointmentStatus.Scheduled, cancellationToken);
        }

        private async Task EnsureAppointmentAsync(
            int clinicId,
            Guid patientId,
            DateTime appointmentDate,
            AppointmentStatus status,
            CancellationToken cancellationToken)
        {
            var exists = await _appointmentRepository.AnyAsync(
                appointment =>
                    appointment.ClinicId == clinicId &&
                    appointment.PatientId == patientId &&
                    appointment.AppointmentDate == appointmentDate,
                cancellationToken);

            if (exists)
            {
                return;
            }

            await _appointmentRepository.AddAsync(new Appointment
            {
                ClinicId = clinicId,
                PatientId = patientId,
                AppointmentDate = appointmentDate,
                CreatedAt = DateTime.Now,
                Status = status
            }, cancellationToken);
        }

        private async Task EnsureMedicalHistoryAsync(Guid doctorId, IReadOnlyList<Patient> patients, CancellationToken cancellationToken)
        {
            foreach (var patient in patients)
            {
                var hasHistory = await _visitsHistoryRepository.AnyAsync(
                    history => history.PatientId == patient.PatientId,
                    cancellationToken);

                if (hasHistory)
                {
                    continue;
                }

                await _visitsHistoryRepository.AddAsync(new VisitsHistory
                {
                    PatientId = patient.PatientId,
                    DoctorId = doctorId,
                    CreatedAt = DateTime.Now.AddDays(-7),
                    PatientComplaint = patient.FullName == "Rania Saleh" ? "Headache and fatigue." : "Follow-up for blood pressure control.",
                    Diagnosis = patient.FullName == "Rania Saleh" ? "Migraine." : "Hypertension.",
                    Notes = "Development seed visit for workflow demos."
                }, cancellationToken);
            }
        }

        private async Task EnsureRolesAsync()
        {
            await EnsureRoleAsync(ApplicationRoles.Doctor);
            await EnsureRoleAsync(ApplicationRoles.Receptionist);
        }

        private async Task EnsureRoleAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return;
            }

            var createRoleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            EnsureIdentitySucceeded(createRoleResult, $"Failed to create role '{roleName}'.");
        }

        private async Task EnsureDoctorUserAsync(Guid doctorId)
        {
            var doctorUser = await FindUserByLoginAsync(DoctorUserName, DoctorEmail);
            if (doctorUser is null)
            {
                doctorUser = new ApplicationUser
                {
                    UserName = DoctorUserName,
                    Email = DoctorEmail,
                    DisplayName = DoctorName,
                    DoctorId = doctorId,
                    IsActive = true,
                    EmailConfirmed = true
                };

                var createDoctorResult = await _userManager.CreateAsync(doctorUser, DoctorPassword);
                EnsureIdentitySucceeded(createDoctorResult, "Failed to create development doctor user.");
            }
            else
            {
                doctorUser.DisplayName = DoctorName;
                doctorUser.DoctorId = doctorId;
                doctorUser.BranchId = null;
                doctorUser.IsActive = true;
                doctorUser.EmailConfirmed = true;

                var updateDoctorResult = await _userManager.UpdateAsync(doctorUser);
                EnsureIdentitySucceeded(updateDoctorResult, "Failed to update development doctor user.");
            }

            await EnsureRoleAssignmentAsync(doctorUser, ApplicationRoles.Doctor);
        }

        private async Task EnsureReceptionistUserAsync(Guid branchId)
        {
            var receptionistUser = await FindUserByLoginAsync(ReceptionistUserName, ReceptionistEmail);
            if (receptionistUser is null)
            {
                receptionistUser = new ApplicationUser
                {
                    UserName = ReceptionistUserName,
                    Email = ReceptionistEmail,
                    DisplayName = ReceptionistDisplayName,
                    BranchId = branchId,
                    IsActive = true,
                    EmailConfirmed = true
                };

                var createReceptionistResult = await _userManager.CreateAsync(receptionistUser, ReceptionistPassword);
                EnsureIdentitySucceeded(createReceptionistResult, "Failed to create development receptionist user.");
            }
            else
            {
                receptionistUser.DisplayName = ReceptionistDisplayName;
                receptionistUser.BranchId = branchId;
                receptionistUser.DoctorId = null;
                receptionistUser.IsActive = true;
                receptionistUser.EmailConfirmed = true;

                var updateReceptionistResult = await _userManager.UpdateAsync(receptionistUser);
                EnsureIdentitySucceeded(updateReceptionistResult, "Failed to update development receptionist user.");
            }

            await EnsureRoleAssignmentAsync(receptionistUser, ApplicationRoles.Receptionist);
        }

        private async Task<ApplicationUser?> FindUserByLoginAsync(string userName, string email)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user is not null)
            {
                return user;
            }

            return await _userManager.FindByEmailAsync(email);
        }

        private async Task EnsureRoleAssignmentAsync(ApplicationUser user, string roleName)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles
                .Where(currentRole => !string.Equals(currentRole, roleName, StringComparison.Ordinal))
                .ToArray();

            if (rolesToRemove.Length > 0)
            {
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                EnsureIdentitySucceeded(removeRolesResult, $"Failed to remove stale roles from user '{user.UserName}'.");
            }

            if (currentRoles.Contains(roleName, StringComparer.Ordinal))
            {
                return;
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);
            EnsureIdentitySucceeded(addToRoleResult, $"Failed to assign role '{roleName}' to user '{user.UserName}'.");
        }

        private static void EnsureIdentitySucceeded(IdentityResult result, string failureMessage)
        {
            if (result.Succeeded)
            {
                return;
            }

            var details = string.Join("; ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"{failureMessage} {details}");
        }
    }
}
