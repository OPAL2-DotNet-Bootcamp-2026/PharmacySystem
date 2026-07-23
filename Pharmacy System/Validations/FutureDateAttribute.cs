using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.Validations
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is not DateTime d || d.Date > DateTime.UtcNow.Date;
        }
    }
}
