using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class CategoryRepo
    {
        private PharmacyContext context;

       
        public CategoryRepo(PharmacyContext _context)
        {
            context = _context;
        }

        // Returns all categories with their medicines
        public List<Category> GetAllCategories()
        {
            return context.Categories
                .Include(c => c.Medicines)
                .ToList();
        }

        // Returns one category using its ID
        public Category? GetCategoryById(int id)
        {
            return context.Categories
                .Include(c => c.Medicines)
                .FirstOrDefault(c => c.CategoryID == id);
        }

        // Returns one category using its name
        public Category? GetCategoryByName(string categoryName)
        {
            return context.Categories
                .FirstOrDefault(c => c.CategoryName == categoryName);
        }

        // Adds a new category
        public void Add(Category category)
        {
            context.Categories.Add(category);
            context.SaveChanges();
        }

        // Saves changes made to an existing category
        public void CategoryUpdate()
        {
            context.SaveChanges();
        }

        // Deletes an existing category
        public void CategoryDelete(Category category)
        {
            context.Categories.Remove(category);
            context.SaveChanges();
        }
    }
}