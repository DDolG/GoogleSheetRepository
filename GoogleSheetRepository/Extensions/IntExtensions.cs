namespace GoogleSheetRepository.Extensions
{
    public  static class IntExtensions
    {
        public static string GetColumnAddressWithHeaderShift(this int propertyNumber)
        {
            var propertyBeginColumn = propertyNumber + Constants.BeginPropertyHeader;
            var letters = ConvertToColumnAddress(propertyBeginColumn);
            return letters.ToString();
        }
                
        private static string ConvertToColumnAddress(this int number)
        {
            var result = string.Empty;
            while((number / Constants.MaxLetterNumber > 0))
            {
                var tmp = number / Constants.MaxLetterNumber;
                result += tmp < Constants.MaxLetterNumber ? ConvertToLetter(tmp) : ConvertToColumnAddress(tmp);
                number -= tmp * Constants.MaxLetterNumber;
            }
            result += ConvertToLetter(number);
            return result;
        }
                
        private static char ConvertToLetter(int number)
        {
            if (number < Constants.MinLetterNumber || number > Constants.MaxLetterNumber)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Number must be between 1 and 26.");
            }
            return (char)(number + 64);
        }

    }
}
