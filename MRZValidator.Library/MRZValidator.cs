namespace MRZValidator.Library
{
    public class PassportMRZValidator
    {
        public MRZData ParseLine(string Line)
        {
            if (string.IsNullOrEmpty(Line) || Line.Length != 44)
                    throw new ArgumentException("Invalid MRZ Line 2. It should be exactly 44 charecters.");
            try
            {
                return new MRZData
                {
                    PassportNumber = Line.Substring(0,9),
                    PassportNumberCheckDigit = Line[9],
                    Nationality = Line.Substring(10, 3),
                    DateOfBirth = ParseDate(Line.Substring(13, 6)),
                    DateOfBirthCheckDigit = Line[19],
                    Sex = Line[20],
                    ExpirationDate = ParseDate(Line.Substring(21, 6)),
                    ExpirationDateCheckDigit = Line[27],
                    PersonalNumber = Line.Substring(28, 14),
                    PersonalNumberCheckDigit = Line[42],
                    FinalLine = Line.Substring(0,43),
                    FinalCheckDigit = Line[43]
                };
            }
            catch(Exception ex) 
            {
                throw new Exception("Error parsing the line.", ex);
            }
        }
        public bool ValidateMRZ(MRZData mrz) 
        {
            
            // Final Check Digit (First 43 characters, excluding the last character which is the check digit)
            bool isFinalCheckDigitValid = CalculateCheckDigit(mrz.FinalLine) == (mrz.FinalCheckDigit - '0');

            // Return true if all check digits are valid
            return ValidatePassport(mrz) && ValidateDOB(mrz) && ValidateExpDate(mrz) && ValidatePersonal(mrz) && isFinalCheckDigitValid;
        }
        // Validate Passport Number Check Digit
        public bool ValidatePassport(MRZData mrz)
        {
            bool isPassportNumberValid = CalculateCheckDigit(mrz.PassportNumber) == (mrz.PassportNumberCheckDigit - '0');
            return isPassportNumberValid;
        }
        // Validate Expiration Date Check Digit
        public bool ValidateExpDate(MRZData mrz)
        {
            bool isExpirationDateValid = CalculateCheckDigit(mrz.ExpirationDate.ToString("yyMMdd")) == (mrz.ExpirationDateCheckDigit - '0');
            return isExpirationDateValid;
        }
        // Validate Date of Birth Check Digit
        public bool ValidateDOB(MRZData mrz)
        {
            bool isDateOfBirthValid = CalculateCheckDigit(mrz.DateOfBirth.ToString("yyMMdd")) == (mrz.DateOfBirthCheckDigit - '0');
            return isDateOfBirthValid;
        }
        // Validate Personal Number Check Digit
        public bool ValidatePersonal(MRZData mrz)
        {
            bool isPersonalNumberDateValid = CalculateCheckDigit(mrz.PersonalNumber) == (mrz.PersonalNumberCheckDigit - '0');
            return isPersonalNumberDateValid;
        }
        public static int CalculateCheckDigit(string value)
        {
            int[] weights = { 7, 3, 1 };
            int sum = 0;
            for(int i=0; i<value.Length; i++)
            {
                int val = GetCharecter(value[i]);
                int weightIndex = i % weights.Length;
                int weight = weights[weightIndex];

                sum += val * weight;
            }

            return sum % 10;
        }
        public static int GetCharecter(char charecter)
        {
            if(char.IsDigit(charecter))
            {
                return charecter - '0';
            }
            else if(char.IsUpper(charecter))
            {
                return charecter - 'A' + 10;
            }
            else
            {
                return 0;
            }
        }
        //parse date
        private DateTime ParseDate(string date)
        {
            return DateTime.ParseExact(date, "yyMMdd", null);
        }
    }
    //public class
    public class MRZData
    {
        public string? PassportNumber { get; set; }
        public char PassportNumberCheckDigit { get; set; }
        public string? Nationality {  get; set; }
        public DateTime DateOfBirth { get; set; }
        public char DateOfBirthCheckDigit { get; set; }
        public char Sex { get; set; }
        public DateTime ExpirationDate { get; set; }
        public char ExpirationDateCheckDigit { get; set; }
        public string? PersonalNumber { get; set; }
        public char PersonalNumberCheckDigit { get; set; }
        public string? FinalLine {  get; set; }
        public char FinalCheckDigit { get; set; }

    }
}
