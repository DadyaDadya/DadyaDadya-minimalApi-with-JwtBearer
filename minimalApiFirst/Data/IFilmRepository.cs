namespace minimalApiFirst.Data
{
    public interface IFilmRepository : IDisposable
    {
        Task<List<Film>> GetFilmsAsync();
        Task<Film> GetFilmAsync(int id);
        Task InsertFilmAsync(Film film);
        Task UpdateFilmAsync(Film film);
        Task DeleteFilmAsync(int id);
        Task SaveAsync();
        Task<List<Film>> FindFilmAsync(string title);
    }
}
