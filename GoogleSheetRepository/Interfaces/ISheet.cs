namespace GoogleSheetRepository.Interfaces
{
    public interface ISheet
    {
        /// <summary>
        /// Check have a page
        /// </summary>
        /// <returns></returns>
        bool HavePage();

        /// <summary>
        /// Create a table
        /// </summary>
        /// <returns></returns>
        void Create();

        /// <summary>
        /// Get page id number
        /// </summary>
        /// <returns></returns>
        int GetSheetId();

        /// <summary>
        /// Get number of last row in table
        /// </summary>
        /// <returns></returns>
        int? GetLastRowNumber();
    }
}