namespace AppointementService.Entity
{
    public class DoctorEntity
    {
        public int DoctorId { get; set; }
        public int DeptId { get; set; }
        public string DoctorName { get; set; }
        public int DoctorAge { get; set; }
        public string DoctorAvailable { get; set; }
        public string Specialization { get; set; }
        public string Qualifications { get; set; }

    }
}
