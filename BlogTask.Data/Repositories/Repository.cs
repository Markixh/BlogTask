using Microsoft.EntityFrameworkCore;

namespace BlogTask.Data.Repositories
{
    /// <summary>
    /// Универсальный репозиторий
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected DbContext _db;

        public DbSet<T> Set { get; private set; }

        public Repository(BlogContext db)
        {
            _db = db;
            var set = _db.Set<T>();
            set.Load();

            Set = set;
        }

        public async Task CreateAsync(T item)
        {           
            await Set.AddAsync(item);
            
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(T item)
        {
            Set.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task<T> GetAsync(Guid id)
        {
            return await Set.FindAsync(id);
        }

        public IEnumerable<T> GetAll()
        {
            return Set;
        }

        public async Task UpdateAsync(T item)
        {
            Set.Update(item);
            await _db.SaveChangesAsync();
        }
    }
}
