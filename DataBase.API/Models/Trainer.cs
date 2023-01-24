namespace DataBase.API.Models
{
    public class Trainer
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DoB { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public int? Weight { get; set; }
        public int? Height { get; set; }
        public string? SportHistory { get; set; }
        public int Speciality { get; set; }
    }
}
