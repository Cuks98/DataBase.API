namespace DataBaseAPI.Models
{
    public class EmployeeResponse
    {
        public int? ErrorId { get; set; }
        public string ErrorMsg { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
