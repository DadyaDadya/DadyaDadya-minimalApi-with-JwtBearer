
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics.Metrics;
using System.Net;

namespace minimalApiFirst.Data
{
    public class FilmRepository : IFilmRepository
    {
        private readonly FilmsDBContext context;
        public FilmRepository(FilmsDBContext context)
        {

            this.context = context;

        }

        public async Task<List<Film>> GetFilmsAsync()
        {
            return await context.Films.ToListAsync();
        }

        public async Task<Film> GetFilmAsync(int id)
        {
            return await context.Films.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task InsertFilmAsync(Film film)
        {
            await context.Films.AddAsync(film);
        }

        public async Task UpdateFilmAsync(Film film)
        {
            var filmFromDb = await context.Films.FindAsync(film.Id);
            if (filmFromDb == null) return;
            filmFromDb.Title = film.Title;
            filmFromDb.Description = film.Description;
            filmFromDb.Rating = film.Rating;
        }

        public async Task DeleteFilmAsync(int id)
        {
            var filmFromDb = await context.Films.FindAsync(id);
            if (filmFromDb == null) return;
            context.Films.Remove(filmFromDb);
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<List<Film>> FindFilmAsync(string title)
        {
            return await context.Films.Where(x => x.Title.Contains(title)).ToListAsync();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                context.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
