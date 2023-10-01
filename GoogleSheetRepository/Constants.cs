namespace GoogleSheetRepository
{
    public static class Constants
    {
        public static string NumberOfPropertyCell { get; } = "A1";
        public static string ColumnForSearchLastRecord { get; } = "B:B";
        public static string ColumnForBeginWriteData { get; } = "B";
        public static string RowForBeginWriteData { get; } = "2";
        public static int RowHeaderNumber { get; } = 1;
        public static string HeaderPropertyStartNameCell { get; } = "B1";
        public static string HeaderPropertyNameRow { get; } = "1";
        public static int BeginPropertyHeader { get; } = 1;
        public static int MaxLetterNumber {get;} = 26;
        public static int MinLetterNumber { get; } = 1;
    }
}
