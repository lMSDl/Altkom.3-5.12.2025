namespace WebAPI.Services
{
    public interface ICrudService<T>
    {
        Task<int> CreateAsync(T entity);
        Task<T?> ReadAsync(int id);
        Task<IEnumerable<T>> ReadAsync();
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }
}
