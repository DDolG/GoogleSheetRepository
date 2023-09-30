namespace GoogleSheetRepository.Interfaces
{
    public interface IGSRepository<T> where T : class
    {
        public Task<List<T>> GetAsync();

        public Task<long> AddAsync(T item);

        public Task<bool> DeleteAsync(T item);

        public Task<bool> UpdateAsync(T oldItem, T newItem);
    }
}