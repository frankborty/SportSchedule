namespace SportSchedule.Services
{
    public class NBAScheduleCSVProcessorService : ICsvProcessorService
    {
        public async Task<List<SportEvent>> ProcessCsvAsync(string csvContent)
        {
            var sportEvents = new List<SportEvent>();

            // Processa il CSV (esempio semplice)
            var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            // Salta l'header (prima riga)
            for (int i = 1; i < lines.Length; i++)
            {
                var columns = lines[i].Split(',');

                if (columns.Length >= 3)
                {
                    DateTime eventDate = DateTimeOffset.Parse(columns[1].Trim()).UtcDateTime;
                    var sportEvent = new SportEvent
                    {
                        Channel = columns[2].Trim('\r').Trim('\n'),
                        Sport = "Basketball",
                        Competition = "NBA",
                        Event = $"{columns[0]}",
                        Time = eventDate,
                    };
                    sportEvents.Add(sportEvent);
                }
            }
            return await Task.FromResult(sportEvents);
        }
    }
}
