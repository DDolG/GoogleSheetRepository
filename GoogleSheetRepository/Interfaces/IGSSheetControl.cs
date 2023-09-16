namespace GoogleSheetRepository.Interfaces
{
    public interface IGSSheetControl
    {
        bool HavePage(string name);

        void Create(string name);
    }
}