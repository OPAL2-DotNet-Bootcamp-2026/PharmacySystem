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

        // Returns all active categories with their medicines
        public async Task<List<Category>> GetAllCategories()
        {
            return await context.categories
                .Where(c => c.IsActive)
                .Include(c => c.Medicines)
                .ToListAsync();
        }

        // Returns one active category using its ID
        public async Task<Category?> GetCategoryById(int id)
        {
            return await context.categories
                .Include(c => c.Medicines)
                .FirstOrDefaultAsync(
                    c => c.CategoryID == id && c.IsActive
                        
                );
        }

        // Returns one category using its name
        public async Task<Category?> GetCategoryByName(string categoryName)
            
        {
            return await context.categories.FirstOrDefaultAsync(c => c.CategoryName == categoryName);
               
            
        }

        // Checks whether the category contains any active medicines
        public async Task<bool> HasActiveMedicines(int categoryId)
        {
            return await context.medicines
                .AnyAsync(m => m.CategoryID == categoryId && m.IsActive );
                        
               
        }

        // Adds a new category
        public async Task Add(Category category)
        {
            await context.categories.AddAsync(category);
            await context.SaveChangesAsync();
        }

        // Saves changes made to  existing category
        public async Task CategoryUpdate()
        {
            await context.SaveChangesAsync();
        }

        // Soft deletes a category
        public async Task CategoryDelete(Category category)
        {
            category.IsActive = false;
            await context.SaveChangesAsync();
        }
    }
}