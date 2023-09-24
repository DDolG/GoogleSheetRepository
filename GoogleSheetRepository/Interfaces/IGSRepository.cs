namespace GoogleSheetRepository.Interfaces
{
    public interface IGSRepository<T> where T : class
    {
        public Task<T> ReadAsync();

        public Task<long> CreateAsync(T item);

        public Task<bool> DeleteAsync(long itemId);

        public Task<bool> UpdateAsync(T item);
    }
}