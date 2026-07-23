using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pharmacy_System.Models;

namespace Pharmacy_System.Modules
{
    

    public class Category : BaseEntity
    {
        // system generated 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID { get; set; }

        // Input: category name entered by the admin
        [Required]
        [MaxLength(100)]
           
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // One category can contain many medicines
        public ICollection<Medicine> Medicines { get; set; }= new List<Medicine>();
            
    }
}