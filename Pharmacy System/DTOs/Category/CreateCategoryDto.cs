using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Category
{
    // Used when creating a new category
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]

        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
        public string? Description { get; set; }
    }
}