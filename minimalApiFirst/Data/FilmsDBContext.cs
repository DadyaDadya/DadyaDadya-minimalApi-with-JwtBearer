public class FilmsDBContext : DbContext
{
    public FilmsDBContext(DbContextOptions<FilmsDBContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    public DbSet<Film> Films { get; set; }
}
