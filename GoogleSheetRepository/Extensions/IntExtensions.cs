namespace GoogleSheetRepository.Extensions
{
    internal static class IntExtensions
    {
        internal static string GetFinishColumn(this int propertyNumber)
        {
            var propertyBeginColumn = propertyNumber + Constants.BeginPropertyHeader;
            var letter = ConvertToLetter(propertyBeginColumn);
            return letter.ToString();
        }

        
        public static char ConvertToLetter(int number)
        {
            if (number < 1 || number > 26)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Number must be between 1 and 26.");
            }
            // ASCII value of 'A' is 65, so we can convert the number to the corresponding character.
            return (char)(number + 64);
        }
        
    }
}
