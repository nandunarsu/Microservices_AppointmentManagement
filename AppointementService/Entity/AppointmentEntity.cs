namespace AppointementService.Entity
{
    public class AppointmentEntity
    {
        public int AppointmentID { get; set; }
        public string PatientName { get; set; }
        public int PatientAge { get; set; }
        public string EmailID { get; set; }
        public string Issue { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public int BookedWith { get; set; } 
        public int BookedBy { get; set; }
        public bool Status { get; set; }
    }
}
