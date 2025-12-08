using System.Collections.Generic;

namespace SportSchedule.Models
{
    public class SportEvent
    {
        public int Id { get; set; }
        public Sport Sport { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTimeOffset Time { get; set; } = DateTimeOffset.Now;
        public List<string> TvChannelList { get; set; } = new List<string>();
        public List<string> StreamingSiteList { get; set; } = new List<string>();
        public string AdditionalInfo { get; set; } = string.Empty;

        public SportEvent(Sport sport, string eventName) { 
            Sport = sport;
            Name = eventName;
        }

        public override string ToString()
        {
            return $"{Sport.Name}, {Name} | {Details} | {Time} | {string.Join(", ", TvChannelList)} | {string.Join(", ", StreamingSiteList)} | {AdditionalInfo}";
        }

        internal static SportEvent SportEventFromString(string inputString)
        {
            var pattern = @"^(?<time>\S+)\s+(?<sport>.+?),\s+(?<competition>.+?):\s+(?<detail>.+?)\s+[\u2013]?\s*(?<channel>.+)$";
            var match = Regex.Match(inputString, pattern);

            if (!match.Success)
            {
                throw new ArgumentException("Formato stringa non valido", nameof(inputString));
            }

            // Parsing dell'orario
            var timeString = match.Groups["time"].Value.Replace('.', ':'); // sostituisce 2.20 -> 2:20
            if (!DateTimeOffset.TryParseExact(timeString, "H:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var time))
            {
                throw new FormatException($"Orario non valido: {timeString}");
            }

            // Imposta la data di oggi con l’orario parsato
            var now = DateTimeOffset.Now;
            var eventTime = new DateTimeOffset(now.Year, now.Month, now.Day, time.Hour, time.Minute, 0, now.Offset);

            Sport sport = new Sport();
            sport.Name = match.Groups["sport"].Value;

            List<string> tvList = new List<string>();
            tvList.Add(match.Groups["channel"].Value);

            SportEvent evento = new SportEvent(sport, match.Groups["competition"].Value)
            {
                Time = eventTime,
                Details = match.Groups["detail"].Value,
                TvChannelList = tvList
            };
            return evento;
        }
    }
}
