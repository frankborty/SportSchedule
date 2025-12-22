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
                var sportEventListQuery = _context.SportEvents.AsNoTracking();
                var result = sportEventListQuery.ToList();
                if (date is not null)
                {
                    sportEventListQuery = sportEventListQuery.Where(e => e.Time.Date == targetDate.Date);
                }

                var sportEventList = await sportEventListQuery.ToListAsync();

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

        // POST: api/SportEvent/AddEvent
        [HttpPost("AddEvent")]
        public async Task<IActionResult> AddEvent([FromBody] SportEvent sportEvent)
        {
            try
            {
                sportEvent.Id = 0;
                if (sportEvent == null)
                {
                    return BadRequest("L'evento sportivo non può essere nullo");
                }

                // Validazione base
                if (string.IsNullOrWhiteSpace(sportEvent.Competition) ||
                    string.IsNullOrWhiteSpace(sportEvent.Event))
                {
                    return BadRequest("Competition ed Event sono campi obbligatori");
                }

                // Controlla se l'evento esiste già nel database
                bool exists = await _context.SportEvents.AnyAsync(e =>
                    e.Competition == sportEvent.Competition &&
                    e.Event == sportEvent.Event &&
                    e.Time == sportEvent.Time &&
                    e.Channel == sportEvent.Channel);

                if (exists)
                {
                    return Conflict("L'evento esiste già nel database");
                }

                // Assicurati che la data sia UTC
                if (sportEvent.Time.Kind != DateTimeKind.Utc)
                {
                    sportEvent.Time = DateTime.SpecifyKind(sportEvent.Time, DateTimeKind.Utc);
                }

                _context.SportEvents.Add(sportEvent);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEvents),
                    new { date = sportEvent.Time.Date },
                    new
                    {
                        Message = "Evento aggiunto con successo",
                        Event = new
                        {
                            sportEvent.Id,
                            sportEvent.Competition,
                            sportEvent.Sport,
                            sportEvent.Event,
                            sportEvent.Time,
                            sportEvent.Channel
                        }
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante l'aggiunta dell'evento: {ex.Message} [{ex.InnerException?.Message}]");
            }
        }

        [HttpPost("AddEvents")]
        public async Task<IActionResult> AddEvents([FromBody] List<SportEvent> sportEvents)
        {
            try
            {
                if (sportEvents == null || !sportEvents.Any())
                {
                    return BadRequest("La lista degli eventi non può essere vuota");
                }

                var addedEvents = new List<SportEvent>();
                var skippedEvents = new List<string>();
                var invalidEvents = new List<string>();

                foreach (var sportEvent in sportEvents)
                {
                    sportEvent.Id = 0;

                    // Validazione base
                    if (string.IsNullOrWhiteSpace(sportEvent.Competition) ||
                        string.IsNullOrWhiteSpace(sportEvent.Event))
                    {
                        invalidEvents.Add($"{sportEvent.Event ?? "N/A"} - {sportEvent.Competition ?? "N/A"}");
                        continue;
                    }

                    // Controlla se l'evento esiste già nel database
                    bool exists = await _context.SportEvents.AnyAsync(e =>
                        e.Competition == sportEvent.Competition &&
                        e.Event == sportEvent.Event &&
                        e.Time == sportEvent.Time &&
                        e.Channel == sportEvent.Channel);

                    if (exists)
                    {
                        skippedEvents.Add($"{sportEvent.Event} - {sportEvent.Competition} - {sportEvent.Time:yyyy-MM-dd HH:mm}");
                        continue;
                    }

                    // Assicurati che la data sia UTC
                    if (sportEvent.Time.Kind != DateTimeKind.Utc)
                    {
                        sportEvent.Time = DateTime.SpecifyKind(sportEvent.Time, DateTimeKind.Utc);
                    }

                    _context.SportEvents.Add(sportEvent);
                    addedEvents.Add(sportEvent);
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Operazione completata",
                    TotalReceived = sportEvents.Count,
                    Added = addedEvents.Count,
                    Skipped = skippedEvents.Count,
                    Invalid = invalidEvents.Count,
                    AddedEvents = addedEvents.Select(e => new
                    {
                        e.Id,
                        e.Competition,
                        e.Sport,
                        e.Event,
                        e.Time,
                        e.Channel
                    }),
                    SkippedEvents = skippedEvents,
                    InvalidEvents = invalidEvents
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante l'aggiunta degli eventi: {ex.Message} [{ex.InnerException?.Message}]");
            }
        }
    }
}
