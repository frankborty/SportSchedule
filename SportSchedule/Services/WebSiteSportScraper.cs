namespace SportSchedule.Services.Scraping
{
    public class WebSiteSportScraper
    {
        static readonly HttpClient client = new HttpClient();

        static CultureInfo culturaItaliana = new CultureInfo("it-IT");

        public WebSiteSportScraper() { }

        public async Task<List<SportEvent>> LoadSportEventFromWebAsync(string rootURL, DateTime date, List<string> regexPatternList)
        {
            string webSiteURL = GetWebSiteSportScheduleURL(rootURL, date);
            List<string> sportEventStringList = await ExtractSportEventFromHTMLAsync(webSiteURL);
            List<SportEvent> result = new List<SportEvent>();
            foreach(var sportEventString in sportEventStringList)
            {
                result.Add(SportEventFromString(sportEventString, regexPatternList, date));
            }
            return result;
        }

        private string GetWebSiteSportScheduleURL(string rootUrl, DateTime? date = null)
        {
            var today = date ?? DateTime.Today;
            string currentYearMonth = $"{today:yyyy/MM}";
            string weekDayMonth = today.ToString("dddd-d-MMMM", culturaItaliana).Replace("ì", "i");
            return $"https://{rootUrl}/{currentYearMonth}/sport-in-tv-oggi-{weekDayMonth}-orari-e-programma-completo-come-vedere-gli-eventi-in-streaming/";
        }

        private async Task<List<string>> ExtractSportEventFromHTMLAsync(string url)
        {
            List<string> htmlNodesResult = new List<string>();
            HtmlDocument htmlDocument = await HTTPHelper.LoadHtmlDocumentFromUrlAsync(client, url);
            HtmlNode div = htmlDocument.GetElementbyId("mvp-content-main");

            if (div != null)
            {
                //prendo solo i nodi p
                HtmlNodeCollection pNodes = HTTPHelper.FiltraNodiP(div);
                foreach (var p in pNodes)
                {
                    string testo = p.InnerText.Trim();
                    if (IsSportEvent(testo))
                    {
                        htmlNodesResult.Add(testo);
                    }
                }
            }
            else
            {
                Console.WriteLine("Div non trovato!");
            }
            return htmlNodesResult;
        }

        private bool IsSportEvent(string htmltext)
        {
            //la stringa inizia con un orario (es: 9.30, 12.50)
            Regex regexOrario = new Regex(@"^\d{1,2}\.\d{2}");
            return regexOrario.IsMatch(htmltext);
        }

        private static SportEvent SportEventFromString(string inputString, List<string> regexPatternList, DateTime date)
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
            string dirtyChannelString = match.Groups["channel"].Value;
            string channelString = CleanChannelString(match.Groups["channel"].Value);

            SportEvent evento = new SportEvent()
            {
                Sport = StringUtils.ConvertToSentenceCase(match.Groups["sport"].Value),
                Competition = StringUtils.ConvertToSentenceCase(match.Groups["competition"].Value),
                Time = eventTime,
                Event = StringUtils.ConvertToSentenceCase(match.Groups["event"].Value),
                Channel = channelString
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

        private static string CleanChannelString(string dirtyChannelString)
        {
            dirtyChannelString = dirtyChannelString.Trim();
            string cleanChannelString = dirtyChannelString.Replace("Diretta TV", " ", StringComparison.InvariantCultureIgnoreCase)
                .Replace("Diretta streaming", " ", StringComparison.InvariantCultureIgnoreCase)
                .Replace("live streaming", " ", StringComparison.InvariantCultureIgnoreCase)
                .Replace(" e ", " ", StringComparison.InvariantCultureIgnoreCase)
                .Replace(" sito ", " ", StringComparison.InvariantCultureIgnoreCase)
                .Replace(" app ", " ", StringComparison.InvariantCultureIgnoreCase)
                .Replace(" su ", " ", StringComparison.InvariantCultureIgnoreCase)
                .Replace(" di ", " ", StringComparison.InvariantCultureIgnoreCase)
                .Replace(".", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace(";", "", StringComparison.InvariantCultureIgnoreCase);

            return Regex.Replace(cleanChannelString, @"\s+", " ").Trim();
        }
    }
}
