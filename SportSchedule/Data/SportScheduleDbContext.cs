namespace SportSchedule.Data
{
    public class SportScheduleDbContext : DbContext
    {
        public SportScheduleDbContext(DbContextOptions<SportScheduleDbContext> options)
        : base(options)
        {
        }
        public DbSet<SportEvent> SportEvents => Set<SportEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SportEvent>()
                .HasIndex(e => new { e.Competition, e.Sport, e.Time, e.Event, e.Channel })
                .IsUnique();
        }
    }
}
