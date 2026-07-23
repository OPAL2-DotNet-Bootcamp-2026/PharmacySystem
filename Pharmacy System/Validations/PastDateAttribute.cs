using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.Validations
{
    public class PastDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateOnly date)
                return date <= DateOnly.FromDateTime(DateTime.UtcNow);
            return true;
        }
    }
}
