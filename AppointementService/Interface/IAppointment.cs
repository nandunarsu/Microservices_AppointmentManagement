using AppointementService.Model;

namespace AppointementService.Interface
{
    public interface IAppointment
    {
      public Task<int> CreateAppointment(AppointmentRequest appointment, int PatientID, int DoctorID);
        // public async Task<int> CreateAppointment(AppointmentRequest appointment, int PatientID, int DoctorID)
       // public async Task<AppointmentRequest> UpdateAppointment(AppointmentRequest request, string? patientId, int AppointmentId)
        
            public Task<IEnumerable<AppointmentRequest>> GetAllAppointmentsByDoctor(int doctorId);
        public Task<IEnumerable<AppointmentRequest>> GetAllAppointmentsByPatient(int patientId);
        public Task<IEnumerable<AppointmentRequest>> GetAppointmentsById(int appointmentId);
        public Task<AppointmentRequest> UpdateAppointment(AppointmentRequest request, int patientId, int AppointmentId);
        public Task<AppointmentRequest> UpdateStatus(int appointmentId, string status);

        public Task<bool> CancelAppointment(int bookedby,int appointmentid);
    }
}
