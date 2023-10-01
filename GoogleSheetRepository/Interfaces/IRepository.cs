namespace GoogleSheetRepository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        List<T> Get();

        List<T> Get(int skip, int take);

        long Add(T item);

        bool Delete(T item);

        bool Update(T oldItem, T newItem);
    }
}