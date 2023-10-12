namespace GoogleSheetRepository.Extensions
{
    public  static class IntExtensions
    {
        /// <summary>
        /// Recalculates the number of class properties to the address of a column in the table
        /// </summary>
        /// <returns></returns>
        public static string GetColumnAddressWithHeaderShift(this int propertyNumber)
        {
            var propertyBeginColumn = propertyNumber + Constants.BeginPropertyHeader;
            var letters = ConvertToColumnAddress(propertyBeginColumn);
            return letters.ToString();
        }

        /// <summary>
        /// Converts a number to a character string
        /// </summary>
        /// <returns></returns>                
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

        /// <summary>
        /// Сonverts one number to a letter
        /// </summary>
        /// <returns></returns>
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
