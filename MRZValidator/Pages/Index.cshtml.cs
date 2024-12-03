using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MRZValidator.Library;

namespace MRZValidator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PassportMRZValidator _validator;

        public IndexModel(PassportMRZValidator validator)
        {
            _validator = validator;
        }
        //User Input for the Fields
        [BindProperty]
        public string MRZLine2 { get; set; } 
        [BindProperty]
        public string PassportNumber { get; set; }
        [BindProperty]
        public string Nationality { get; set; }
        [BindProperty]
        public DateTime DOB { get; set; }
        [BindProperty]
        public char Gender { get; set; }
        [BindProperty]
        public DateTime ExpDate { get; set; }

        public MRZData ParsedMRZ { get; private set; } // Parsed MRZ data
        public bool? IsMRZValid { get; private set; } // Validation result (null means not validated yet)
        public Dictionary<string, bool> ValidationResults { get; private set; }// To store the data entered 
        public string ErrorMessage { get; private set; } // To display error messages
        public void OnGet()
        {
            // Initial page load
        }

        public void OnPostValidate()
        {
            ValidationResults = new Dictionary<string, bool>(); 

            if (string.IsNullOrWhiteSpace(MRZLine2))
            {
                ErrorMessage = "MRZ Line 2 cannot be empty.";
                return;
            }
            if (Gender != 'M' && Gender != 'F' && Gender != '0')
            {
                ErrorMessage = "Please select a gender.";
                return;
            }
            if (!string.IsNullOrEmpty(Nationality))
            {
                Nationality = Nationality.ToUpper();
            }
            if (!string.IsNullOrEmpty(MRZLine2))
            {
                MRZLine2 = MRZLine2.ToUpper();
            }
            try
            {
                // Try to parse the MRZ Line 2 input
                try
                {
                    ParsedMRZ = _validator.ParseLine(MRZLine2);
                }
                catch (Exception parseException)
                {
                    // Catch parse exceptions but allow validation to continue
                    ErrorMessage = parseException.Message;
                }

                if (ParsedMRZ != null)
                {
                    // Perform individual field validations only if MRZ is parsed successfully.
                    ValidationResults["Passport Number"] = ParsedMRZ.PassportNumber == PassportNumber;
                    ValidationResults["Nationality"] = ParsedMRZ.Nationality == Nationality;
                    ValidationResults["Date of Birth"] = ParsedMRZ.DateOfBirth == DOB;
                    ValidationResults["Gender"] = ParsedMRZ.Sex == Gender;
                    ValidationResults["Expiry Date"] = ParsedMRZ.ExpirationDate == ExpDate;

                    // Add check digit validations
                    ValidationResults["Passport Check"] = _validator.ValidatePassport(ParsedMRZ);
                    ValidationResults["DOB Check"] = _validator.ValidateDOB(ParsedMRZ);
                    ValidationResults["Expiry Date Check"] = _validator.ValidateExpDate(ParsedMRZ);
                    ValidationResults["Personal Number Check"] = _validator.ValidatePersonal(ParsedMRZ);

                    // Validate MRZ Line 2
                    IsMRZValid = _validator.ValidateMRZ(ParsedMRZ);
                    ValidationResults["MRZ Line 2 Valid"] = IsMRZValid.Value;
                    
                }
                else
                {
                    // If parsing failed, mark all validations as false
                    ValidationResults["Passport Number"] = false;
                    ValidationResults["Nationality"] = false;
                    ValidationResults["Date of Birth"] = false;
                    ValidationResults["Gender"] = false;
                    ValidationResults["Expiry Date"] = false;
                    ValidationResults["Passport Check"] = false;
                    ValidationResults["DOB Check"] = false;
                    ValidationResults["Expiry Date Check"] = false;
                    ValidationResults["Personal Number Check"] = false;
                    ValidationResults["MRZ Line 2 Valid"] = false; 
                }
            }
            catch (Exception ex)
            {
                // Catch unexpected errors and display an error message
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
            }
        }
        //check if all values are true or false
        public bool AreAllValid()
        {
            return ValidationResults.Values.All(valid => valid);
        }
    }
}