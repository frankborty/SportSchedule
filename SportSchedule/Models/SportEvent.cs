namespace SportSchedule.Models
{
    public class SportEvent
    {
        public int Id { get; set; }
        public string Sport { get; set; } = string.Empty;
        public string Competition { get; set; } = string.Empty;
        public string Event { get; set; } = string.Empty;
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public string Channel { get; set; } = string.Empty;
        public string AdditionalInfo { get; set; } = string.Empty;


        public override string ToString()
        {
            return $"Sport: {Sport} | {Competition} | {Event} | {Time} | {Channel} | {AdditionalInfo}";
        }
    }
}
