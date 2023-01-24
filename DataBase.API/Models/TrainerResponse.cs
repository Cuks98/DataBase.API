using DataBase.API.Models;

namespace DataBaseAPI.Models
{
    public class TrainerResponse
    {
        public int? ErrorId { get; set; }
        public string ErrorMsg { get; set; }
        public List<Trainer> Trainers { get; set; }
    }
}
