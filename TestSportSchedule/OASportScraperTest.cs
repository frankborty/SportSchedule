using SportSchedule.Services.Scraping;

namespace TestSportSchedule
{
    public class WebSiteSportScraperTest
    {
        [Theory]
        [InlineData(2025, 12, 8, "https://www.xyz.it/2025/12/sport-in-tv-oggi-lunedi-8-dicembre-orari-e-programma-completo-come-vedere-gli-eventi-in-streaming/")]
        [InlineData(2025, 12, 7, "https://www.xyz.it/2025/12/sport-in-tv-oggi-domenica-7-dicembre-orari-e-programma-completo-come-vedere-gli-eventi-in-streaming/")]
        [InlineData(2025, 1, 11, "https://www.xyz.it/2025/1/sport-in-tv-oggi-sabato-11-gennaio-orari-e-programma-completo-come-vedere-gli-eventi-in-streaming/")]
        [InlineData(2026, 1, 1, "https://www.xyz.it/2026/1/sport-in-tv-oggi-giovedi-1-gennaio-orari-e-programma-completo-come-vedere-gli-eventi-in-streaming/")]
        public async Task WebSiteURLCreation(int year, int month, int day, string expectedResult)
        {
            DateTime date = new DateTime(year, month, day);
            string oaUrl = new WebSiteSportScraper().GetWebSiteSportScheduleURL(date);
            Assert.Equal(expectedResult, oaUrl);
        }

        [Fact]
        public async Task ExtractSportEventFromHTML()
        {
            DateTime date = new DateTime(2025, 12, 8);
            var webSportScraper = new WebSiteSportScraper();
            string oaUrl = webSportScraper.GetWebSiteSportScheduleURL(date);
            List<string> events = await webSportScraper.ExtractSportEventFromHTMLAsync(oaUrl);
            Assert.NotEmpty(events);
            foreach (var sportEvent in events)
            {
                Console.WriteLine(sportEvent);
            }
        }

        [Fact]
        public async Task GetWebSportScheduleURL()
        {
            var webSportScraper = new WebSiteSportScraper();
            var events = await webSportScraper.LoadSportEventFromWeb();
            Assert.NotEmpty(events);
            foreach (var sportEvent in events)
            {
                Console.WriteLine(sportEvent);
            }
        }
    }
}
