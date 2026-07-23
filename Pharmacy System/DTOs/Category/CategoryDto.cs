namespace Pharmacy_System.DTOs.Category
{
    // Used to return category information from the API
    public class CategoryDto
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

    }
}