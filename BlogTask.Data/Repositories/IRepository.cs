public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    Task<T> GetAsync(Guid id);
    Task CreateAsync(T item);
    Task UpdateAsync(T item);
    Task DeleteAsync(T item);
}
