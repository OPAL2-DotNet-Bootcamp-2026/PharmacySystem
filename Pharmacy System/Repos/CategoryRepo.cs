using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class CategoryRepo
    {
        private readonly PharmacyContext context;

        public CategoryRepo(PharmacyContext context)
        {
            this.context = context;
        }

        // Returns all categories with their medicines
        public async Task<List<Category>> GetAllCategories()
        {
            return await context.categories
                .Include(c => c.Medicines)
                .ToListAsync();
        }

        // Returns one category using its ID
        public async Task<Category?> GetCategoryById(int id)
        {
            return await context.categories
                .Include(c => c.Medicines)
                .FirstOrDefaultAsync(c => c.CategoryID == id);
        }

        // Returns one category using its name
        public async Task<Category?> GetCategoryByName(string categoryName)
        {
            return await context.categories
                .FirstOrDefaultAsync(
                    c => c.CategoryName == categoryName
                );
        }

        // Adds a new category
        public async Task Add(Category category)
        {
            await context.categories.AddAsync(category);
            await context.SaveChangesAsync();
        }

        // Saves changes made to a category
        public async Task CategoryUpdate()
        {
            await context.SaveChangesAsync();
        }

        // Deletes a category
        public async Task CategoryDelete(Category category)
        {
            context.categories.Remove(category);
            await context.SaveChangesAsync();
        }
    }
}