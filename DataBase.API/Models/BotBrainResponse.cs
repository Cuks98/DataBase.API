namespace DataBaseAPI.Models
{
    public class BotBrainResponse
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<Dictionary<string, string>> Data { get; set; }
    }
}
