using AppointementService.Conetxt;
using AppointementService.Entity;
using AppointementService.Interface;
using AppointementService.Model;
using Dapper;
using System.Data;
using System.Net.Http;

namespace AppointementService.Services
{
    public class AppointmentServices : IAppointment
    {
        private readonly DapperContext _context;
        private readonly IHttpClientFactory httpClientFactory;

        public AppointmentServices(DapperContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<int> CreateAppointment(AppointmentRequest appointment, int PatientID, int DoctorID)
        {
            try
            {
                string insertQuery = @"INSERT INTO APPOINTMENT (PatientName, PatientAge, Issue, DoctorName, Specialization,
                              AppointmentDate, Status, BookedWith, BookedBy)
                              VALUES (@PatientName, @PatientAge, @Issue, @DoctorName, @Specialization,
                              @AppointmentDate, @Status, @BookedWith, @BookedBy);";
                AppointmentEntity appointmentEntity = MapToEntity(appointment, PatientID, getDoctorById(DoctorID));
                using (var connection = _context.CreateConnection())
                {
                    var appointmentId = await connection.ExecuteAsync(insertQuery, appointmentEntity);

                    return appointmentId;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        private AppointmentEntity MapToEntity(AppointmentRequest request, int PatientId, DoctorEntity userObject)
        {
            return new AppointmentEntity
            {
                PatientName = request.PatientName,
                PatientAge = request.PatientAge,
                Issue = request.Issue,
                DoctorName = userObject?.DoctorName ?? "", // Ensure userObject is not null
                Specialization = userObject?.Specialization ?? "", // Ensure userObject is not null
                AppointmentDate = DateTime.Now,
                Status = false,
                BookedWith = userObject?.DoctorId ?? 0, // Ensure userObject is not null
                BookedBy = PatientId
            };
        }
        public DoctorEntity getDoctorById(int doctorId)
        {
            try
            {
                var httpclient = httpClientFactory.CreateClient("GetDoctorById");
                var response = httpclient.GetAsync($"GetDoctorById?doctorId={doctorId}").Result;

                if (response.IsSuccessStatusCode)
                {
                   
                    //Console.WriteLine(response.Content.ReadFromJsonAsync<DoctorEntity>().Result);

                    var result= response.Content.ReadFromJsonAsync<DoctorEntity>().Result;
                    Console.WriteLine(result.DoctorName);
                    return result;
                }
                else
                {
                    Console.WriteLine(response);
                    throw new Exception("Doctor not found. Please check the provided DoctorID.");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"An error occurred while fetching doctor details: {e.Message}");
            }
        }
        public async Task<IEnumerable<AppointmentRequest>> GetAllAppointmentsByPatient(int patientId)
        {
            try
            {
                string selectQuery = @"SELECT * FROM APPOINTMENT WHERE BookedBy = @PatientId;";
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<AppointmentRequest>(selectQuery, new { PatientId = patientId });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving appointments by patient.", ex);
            }
        }

        public async Task<IEnumerable<AppointmentRequest>> GetAllAppointmentsByDoctor(int doctorId)
        {
            try
            {
                string selectQuery = @"SELECT * FROM APPOINTMENT WHERE BookedWith = @DoctorId;";
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<AppointmentRequest>(selectQuery, new { DoctorId = doctorId });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving appointments by doctor{ex.Message}");
            }
        }

        public async Task<IEnumerable<AppointmentRequest>> GetAppointmentsById(int appointmentId)
        {
            try
            {
                string selectQuery = @"SELECT * FROM APPOINTMENT WHERE AppointmentId = @AppointmentId;";
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<AppointmentRequest>(selectQuery, new { AppointmentId = appointmentId });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving appointments by doctor.{ex.Message}");
            }
        }

        public async Task<AppointmentRequest> UpdateAppointment(AppointmentRequest request, int patientId, int AppointmentId)
        {
            AppointmentEntity existingAppointment = GetAppointmentsbyId(AppointmentId);
            if (existingAppointment == null)
            {
                throw new Exception("Appointment not found");
            }
            existingAppointment.PatientName = request.PatientName;
            existingAppointment.PatientAge = request.PatientAge;
            existingAppointment.Issue = request.Issue;
            existingAppointment.AppointmentDate = request.AppointmentDate;

            string sql = @" UPDATE APPOINTMENT SET PatientName = @PatientName, 
        PatientAge = @PatientAge, Issue = @Issue, BookedBy = @BookedBy, AppointmentDate = @AppointmentDate WHERE AppointmentId = @AppointmentId";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(sql, existingAppointment);

                // Assuming AppointmentResponseDto needs to be retrieved after the update
                return await connection.QueryFirstOrDefaultAsync<AppointmentRequest>("SELECT * FROM APPOINTMENT WHERE AppointmentId = @AppointmentId", new { AppointmentId = AppointmentId });
            }
        }


        public async Task<AppointmentRequest> UpdateStatus(int appointmentId, string status)
        {
            string query = "UPDATE APPOINTMENT SET Status = @Status WHERE AppointmentId = @AppointmentId";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Status = status, AppointmentId = appointmentId });

                // Assuming AppointmentResponseDto needs to be retrieved after the update
                return await connection.QueryFirstOrDefaultAsync<AppointmentRequest>("SELECT * FROM APPOINTMENT WHERE AppointmentId = @AppointmentId", new { AppointmentId = appointmentId });
            }
        }

        private AppointmentEntity GetAppointmentsbyId(int appointmentId)
        {
            try
            {
                string selectQuery = @"SELECT * FROM APPOINTMENT WHERE AppointmentId = @AppointmentId;";
                using (var connection = _context.CreateConnection())
                {
                    return connection.QueryFirstOrDefault<AppointmentEntity>(selectQuery, new { AppointmentId = appointmentId });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving appointment by ID.", ex);
            }
        }

        public async Task<bool> CancelAppointment(int bookedby, int appointmentid)
        {
            try
            {
                string deleteQuery = @"DELETE FROM APPOINTMENT WHERE AppointmentId = @AppointmentId AND BookedBy = @BookedBy;";

                using (var connection = _context.CreateConnection())
                {
                    int rowsAffected = await connection.ExecuteAsync(deleteQuery, new { AppointmentId = appointmentid, BookedBy = bookedby });
                    return rowsAffected > 0; 
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting appointment by ID.", ex);
            }
        }



    }
}
