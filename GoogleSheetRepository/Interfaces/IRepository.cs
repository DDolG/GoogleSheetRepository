namespace GoogleSheetRepository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAsync();

        Task<List<T>> GetAsync(int skip, int take);

        Task<long> AddAsync(T item);

        Task<bool> DeleteAsync(T item);

        Task<bool> UpdateAsync(T oldItem, T newItem);
    }
}