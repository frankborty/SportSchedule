namespace SportSchedule.Services
{
    public interface ICsvProcessorService
    {
        Task<List<SportEvent>> ProcessCsvAsync(string csvContent);
    }
}
