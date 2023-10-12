using GoogleSheetRepository.Models;

namespace GoogleSheetRepository.Interfaces
{
    public interface IHeader
    {
        /// <summary>
        /// Get the number of properties recorded in the table
        /// </summary>
        /// <returns></returns>
        List<ColumnPropertyHeader> GetPropertiesFromHeader();

        /// <summary>
        /// Get a list of properties from the table header
        /// </summary>
        /// <returns></returns>
        int? GetPropertyCountFromPage();

        /// <summary>
        /// Storing the number of properties in the table header
        /// </summary>
        /// <returns></returns>
        void SetPropertyCount();

        /// <summary>
        /// Initializing property column headers
        /// </summary>
        /// <returns></returns>
        void InitPropertyHeaders();
    }
}
