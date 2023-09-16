namespace GoogleSheetRepository.Interfaces
{
    public interface IGSRepository<T> where T : class
    {
        Task<T> GetAsync();

        Task<long> AddAsync(T item);

        Task<bool> DeleteAsync(long itemId);

        Task<bool> UpdateAsync(T item);
    }
}