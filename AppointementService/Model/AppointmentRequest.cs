namespace AppointementService.Model
{
    public class AppointmentRequest
    {
        public string PatientName { get; set; }
        public int PatientAge { get; set; }
        public string Issue { get; set; }

        public DateTime AppointmentDate { get; set; }
    }
}
