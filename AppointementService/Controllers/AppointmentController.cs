using AppointementService.Interface;
using AppointementService.Model;
using AppointementService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppointementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointment _appointment;
        private readonly HttpClient _httpClient;

        public AppointmentController(IAppointment appointment, HttpClient httpClient)
        {
            _appointment = appointment;
            _httpClient = httpClient;
        }
        [Authorize(Roles = "Patient")]
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentRequest appointment, int DoctorID)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                int patientId = int.Parse(userIdClaim);
                Console.WriteLine(patientId);
                var addedAppointment = await _appointment.CreateAppointment(appointment, patientId, DoctorID);
                return Ok(new { Success = true, Message = "Appointment added successfully", Data = addedAppointment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("GetByPatient")]
        public async Task<IActionResult> GetAllAppointmentsByPatient()
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                int patientId = int.Parse(userIdClaim);

                Console.WriteLine(patientId);

                var appointments = await _appointment.GetAllAppointmentsByPatient(patientId);
                return Ok(new { Success = true, Message = "Appointment details:", Data = appointments });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("GetByDoctor")]
        public async Task<IActionResult> GetAllAppointmentsByDoctor()
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                int doctorId = int.Parse(userIdClaim);
                var appointments = await _appointment.GetAllAppointmentsByDoctor(doctorId);
                return Ok(new { Success = true, Message = "Appointment details", Data = appointments });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }

        [Authorize(Roles = "Patient,Doctor,Admin")]
        [HttpGet("GetAppointmentById")]
        public async Task<IActionResult> GetAppointmentsById(int Appointmentid)
        {
            try
            {
                //var userIdClaim = User.FindFirstValue("UserId");
                //int userId = int.Parse(userIdClaim);
                var appointments = await _appointment.GetAppointmentsById(Appointmentid);
                return Ok(new { Success = true, Message = "Appointment details:", Data = appointments });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpPut("UpdateAppointment")]
        public async Task<IActionResult> UpdateAppointment(AppointmentRequest request, int AppointmentId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                int userId = int.Parse(userIdClaim);
                var updatedAppointment = await _appointment.UpdateAppointment(request, userId, AppointmentId);

                var response = new ResponseModel<AppointmentRequest>
                {
                    Success = true,
                    Message = "Appointment updated successfully",
                    Data = updatedAppointment
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }


        [Authorize(Roles = "Doctor")]
        [HttpPut("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(int Appointmentid, string status)
        {
            try
            {
                var updatedAppointment = await _appointment.UpdateStatus(Appointmentid, status);

                if (updatedAppointment != null)
                {
                    return Ok(new ResponseModel<AppointmentRequest>
                    {
                        Success = true,
                        Message = "Appointment status updated successfully",
                        Data = updatedAppointment
                    });
                }
                else
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Appointment not found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }


        [Authorize(Roles = "Patient")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAppointment(int Appointmentid)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                int patientid = int.Parse(userIdClaim);

                bool cancellationResult = await _appointment.CancelAppointment(patientid, Appointmentid);

                if (cancellationResult)
                {
                    return Ok(new { Success = true, Message = "Appointment successfully cancelled." });
                }
                else
                {
                    return NotFound(new { Success = false, Message = "Appointment not found or could not be cancelled." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }



    }
}
