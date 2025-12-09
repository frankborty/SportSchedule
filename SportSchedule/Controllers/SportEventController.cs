namespace SportSchedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportEventController : ControllerBase
    {
        private readonly SportScheduleDbContext _context;
        private readonly IConfiguration _configuration;

        // Inietti sia DbContext che IConfiguration
        public SportEventController(SportScheduleDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/SportEvent/LoadEvents
        [HttpPost("LoadEvents")]
        public async Task<IActionResult> LoadEvents(DateTime? date = null)
        {
            DateTime targetDate = date ?? DateTime.UtcNow.Date;
            // Leggi eventuali impostazioni da appsettings.json
            try
            {
                List<string> regexPatterns = _configuration.GetSection("SportEventRegexPatterns").Get<List<string>>() ?? throw new Exception("SportEventRegexPatterns not found");
                string rootURL = _configuration.GetValue<string>("WebSiteRootUrl") ?? throw new Exception("WebSiteRootUrl not found");
                var wsSportScraper = new WebSiteSportScraper();
                List<SportEvent> sportEventList = await wsSportScraper.LoadSportEventFromWebAsync(rootURL, targetDate, regexPatterns);
                foreach(var sportEvent in sportEventList)
                {
                    // Controlla se l'evento esiste già nel database
                    bool exists = await _context.SportEvents.AnyAsync(e =>
                        e.Competition == sportEvent.Competition &&
                        e.Event == sportEvent.Event &&
                        e.Time == sportEvent.Time &&
                        e.Channel == sportEvent.Channel);
                    if (!exists)
                    {
                        _context.SportEvents.Add(sportEvent);
                    }
                }
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "SportEvent di default aggiunto con successo",
                    Events = sportEventList.Select(e => new { e.Competition, e.Sport, e.Event, e.Time, e.Channel })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante il caricamento eventi: {ex.Message} [{ex.InnerException?.Message}]");
            }
        }


        // GET: api/SportEvent/GetEvents
        [HttpGet("GetEvents")]
        public async Task<IActionResult> GetEvents([FromQuery] DateTime? date = null)
        {
            DateTime targetDate = date ?? DateTime.UtcNow.Date;
            // Leggi eventuali impostazioni da appsettings.json
            try
            {
                var sportEventList = await _context.SportEvents
                    .Where(e => e.Time.Date == targetDate)
                    .ToListAsync();

                if (!sportEventList.Any())
                {
                    return NotFound($"Nessun evento trovato per la data {targetDate:yyyy-MM-dd}");
                }

                return Ok(sportEventList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante il caricamento eventi: {ex.Message} [{ex.InnerException?.Message}]");
            }
        }
    }
}
