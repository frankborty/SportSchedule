using SportSchedule.Models;
using SportSchedule.Services.Scraping;
using SportSchedule.Utils;

List<SportEvent> result = new List<SportEvent>();
List<SportEvent> sportEventList = new List<SportEvent>();

List<DateTime> sportDateStringList = new List<DateTime>
{
    new DateTime(2025, 5, 1),
    new DateTime(2025, 5, 2),
    new DateTime(2025, 5, 3),
    new DateTime(2025, 5, 4),
    new DateTime(2025, 5, 5),
    new DateTime(2025, 5, 6),
    new DateTime(2025, 5, 7),
    new DateTime(2025, 5, 8),
    new DateTime(2025, 5, 9),

    new DateTime(2025, 12, 1),
    new DateTime(2025, 12, 2),
    new DateTime(2025, 12, 3),
    new DateTime(2025, 12, 4),
    new DateTime(2025, 12, 5),
    new DateTime(2025, 12, 6),
    new DateTime(2025, 12, 7),
    new DateTime(2025, 12, 8),
    new DateTime(2025, 12, 9)
};

foreach (var date in sportDateStringList)
{
    //sportChannelStringList.AddRange(await GetListOfChannelsName(date));
    //sportNameStringList = sportNameStringList.Distinct().ToList();
    sportEventList.AddRange(await GetListOfSportEvent(date));
}

//sportNameStringList = sportNameStringList.Distinct().ToList();
//Console.WriteLine(string.Join("\", \"", sportNameStringList));

//sportChannelStringList = sportChannelStringList.Distinct().ToList();
//Console.WriteLine(string.Join("\", \"", sportChannelStringList));

sportEventList = sportEventList.Distinct().ToList();
foreach (var sportEvent in sportEventList)
{
    ;// Console.WriteLine(sportEvent);
}

async Task<List<SportEvent>> GetListOfSportEvent(DateTime date)
{
    try
    {
        var wsSS = new WebSiteSportScraper();
        string todayURL = wsSS.GetWebSiteSportScheduleURL(date);
        List<string> sportEventStringList = await wsSS.ExtractSportEventFromHTMLAsync(todayURL);

        List<string> regexPatternList = new List<string>
        {
            @"^(?<time>\S+)\s+(?<sport>.+?),\s+(?<competition>.+?):\s+(?<detail>.+?)\s+[\u2013-]\s*(?<channel>.+)$",
            @"^(?<time>\S+)\s+(?<sport>[^\(]+)\((?<competition>[^\)]+)\)\s*[\u2013-]\s*(?<event>[^\(]+)\((?<channel>.+)\)$",
            @"^(?<time>\S+)\s+(?<sport>.+?)\s*[\u2013-]\s*(?<competition>.+?),\s*(?<event>[^\(]+)\s*\((?<channel>.+)\)$",
            @"^(?<time>\S+)\s+(?<sport>.+?),\s*(?<competition>.+?)\s*[\u2013-]\s*(?<channel>.+)$",
            @"^(?<time>\S+)\s+(?<sport>.+?)\s*[\u2013-]\s*(?<competition>.+?)\s*\((?<channel>.+)\)$"
        };

        foreach (var sportEventString in sportEventStringList)
        {
            result.Add(TmpUtils.SportEventFromString(sportEventString, regexPatternList, DateTime.UtcNow));
        }

        return result;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Errore durante l'elaborazione della data {date}: {ex.Message}");
        return new List<SportEvent>();
    }
}