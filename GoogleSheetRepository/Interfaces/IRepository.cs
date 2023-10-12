namespace GoogleSheetRepository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Get all records
        /// </summary>
        /// <returns></returns>
        List<T> Get();

        /// <summary>
        /// Get pagination list of record
        /// </summary>
        /// <returns></returns>
        List<T> Get(int skip, int take);

        /// <summary>
        /// Add record to the end of the table
        /// </summary>
        /// <returns></returns>
        long Add(T item);

        /// <summary>
        /// Delete record
        /// </summary>
        /// <returns></returns>
        bool Delete(T item);

        /// <summary>
        /// Update record
        /// </summary>
        /// <returns></returns>
        bool Update(T oldItem, T newItem);
    }
}