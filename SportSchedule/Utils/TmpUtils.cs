namespace SportSchedule.Utils
{
    public class TmpUtils
    {
        public static SportEvent SportEventFromString(string inputString, List<string> regexPatternList, DateTime date)
        {

            Match match = ExtractMatchFromInputString(inputString, regexPatternList);
            if (match == Match.Empty)
            {
                throw new ArgumentException("Formato stringa non valido", inputString);
            }

            // Parsing dell'orario
            var timeString = match.Groups["time"].Value.Replace('.', ':'); // sostituisce 2.20 -> 2:20
            timeString = Regex.Replace(timeString, @"[^0-9:]", ""); // rimuove tutto tranne numeri e :
            if (!DateTimeOffset.TryParseExact(timeString, "H:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var time))
            {
                throw new FormatException($"Orario non valido: {timeString}");
            }

            // Imposta la data di oggi con l’orario parsato
            var now = DateTimeOffset.Now;
            DateTime eventTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0).ToUniversalTime();
            List<string> tvList = new List<string>();
            tvList.Add(match.Groups["channel"].Value);

            SportEvent evento = new SportEvent()
            {
                Sport = StringUtils.ConvertToSentenceCase(match.Groups["sport"].Value),
                Competition = StringUtils.ConvertToSentenceCase(match.Groups["competition"].Value),
                Time = eventTime,
                Event = StringUtils.ConvertToSentenceCase(match.Groups["event"].Value),
                Channel = match.Groups["channel"].Value
            };
            return evento;
        }

        private static Match ExtractMatchFromInputString(string inputString, List<string> regexPatternList)
        {
            string input = WebUtility.HtmlDecode(inputString);
            Match resultMatch;
            foreach (var pattern in regexPatternList)
            {
                resultMatch = Regex.Match(input, pattern);
                if (resultMatch.Success)
                {
                    return resultMatch;
                }
            }
            return Match.Empty;
        }

    }
}
