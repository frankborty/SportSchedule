namespace SportSchedule.Services.Scraping
{
    public class WebSiteSportScraper
    {
        static readonly HttpClient client = new HttpClient();

        static CultureInfo culturaItaliana = new CultureInfo("it-IT");

        public WebSiteSportScraper() { }

        public async Task<List<SportEvent>> LoadSportEventFromWeb()
        {
            string todayURL = GetWebSiteSportScheduleURL();
            List<string> sportEventStringList = await ExtractSportEventFromHTMLAsync(todayURL);
            List<SportEvent> result = new List<SportEvent>();
            foreach(var sportEventString in sportEventStringList)
            {
                result.Add(SportEvent.SportEventFromString(sportEventString));
            }
            return result;
        }

        public string GetWebSiteSportScheduleURL(DateTime? date = null)
        {
            var today = date ?? DateTime.Today;
            string currentYearMonth = $"{today:yyyy/MM}";
            string weekDayMonth = today.ToString("dddd-d-MMMM", culturaItaliana).Replace("ì", "i");
            return $"https://www.xyz.it/{currentYearMonth}/sport-in-tv-oggi-{weekDayMonth}-orari-e-programma-completo-come-vedere-gli-eventi-in-streaming/";
        }

        public async Task<List<string>> ExtractSportEventFromHTMLAsync(string url)
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
    }
}
