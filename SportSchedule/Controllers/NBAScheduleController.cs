namespace SportSchedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NBAScheduleController : ControllerBase
    {
        private readonly ICsvProcessorService _csvProcessor;
        private readonly SportScheduleDbContext _context;

        // Inietti sia DbContext che IConfiguration
        public NBAScheduleController(SportScheduleDbContext context, ICsvProcessorService csvProcessor)
        {
            _csvProcessor = csvProcessor;
            _context = context;
        }

        [HttpPost("UploadNbaScheduleCsv")]
        public async Task<IActionResult> UploadNbaScheduleCsv(IFormFile file)
        {
            // Validazione file
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nessun file caricato");
            }

            // Verifica estensione
            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Il file deve essere in formato CSV");
            }

            try
            {
                // Leggi il contenuto del file
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var csvContent = await reader.ReadToEndAsync();

                // Processa il CSV
                var sportEventList = await _csvProcessor.ProcessCsvAsync(csvContent);

                //aggiungo al db quelle che non esistono già
                foreach (var sportEvent in sportEventList)
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
                    message = "File processato con successo",
                    recordsProcessed = sportEventList.Count,
                    data = sportEventList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore: {ex.Message}");
            }
        }


        // GET: api/SportEvent/GetEvents
        [HttpGet("GetNBAEvents")]
        public async Task<IActionResult> GetNBAEvents([FromQuery] DateTime? date = null)
        {
            DateTime targetDate = date ?? DateTime.UtcNow.Date;
            // Leggi eventuali impostazioni da appsettings.json
            try
            {
                var sportEventListQuery = _context.SportEvents.AsNoTracking();

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
    }
}
