using Pharmacy_System.DTOs.Category;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class CategoryService
    {
        private readonly CategoryRepo categoryRepo;

        public CategoryService(CategoryRepo categoryRepo)
        {
            this.categoryRepo = categoryRepo;
        }

        // Returns all active categories 
        public async Task<List<CategoryDto>> GetAllCategories()
        {
            List<Category> categories = await categoryRepo.GetAllCategories();
               

            List<CategoryDto> categoryDtos =  new List<CategoryDto>();

              
            foreach (Category category in categories)
            {
                CategoryDto dto = ConvertToDto(category);
                categoryDtos.Add(dto);
            }

            return categoryDtos;
        }

        // Returns one active category using its ID
        public async Task<CategoryDto?> GetCategoryById(int id)
        {
            Category? category =  await categoryRepo.GetCategoryById(id);
              

            if (category == null)
            {
                return null;
            }

            return ConvertToDto(category);
        }

        // Creates a new category
        public async Task<int> CreateCategory( CreateCategoryDto dto)
           
        {
            string categoryName =dto.CategoryName.Trim();


            Category? existingCategory = await categoryRepo.GetCategoryByName(categoryName);


            if (existingCategory != null)
            {
                throw new Exception(  "Category name already exists" );
                  

            }

            Category category = new Category
            {
                CategoryName = categoryName,
                Description = dto.Description?.Trim(),

                // New categories are active by default
                IsActive = true
            };

            await categoryRepo.Add(category);

            return category.CategoryID;
        }

        // Updates an existing category
        public async Task<bool> UpdateCategory( int id,  UpdateCategoryDto dto)
         
         
        {
            Category? category =  await categoryRepo.GetCategoryById(id);
              

            if (category == null)
            {
                return false;
            }

            string categoryName =dto.CategoryName.Trim();


            Category? existingCategory = await categoryRepo.GetCategoryByName(categoryName);
               



            if (existingCategory != null &&
                existingCategory.CategoryID != id)
            {
                throw new Exception( "Category name already exists" );

               
            }

            category.CategoryName = categoryName;
            category.Description = dto.Description?.Trim();
               

            await categoryRepo.CategoryUpdate();

            return true;
        }

       
        public async Task<bool> DeleteCategory(int id)
        {
            Category? category =await categoryRepo.GetCategoryById(id);
                

            if (category == null)
            {
                return false;
            }

            bool hasMedicines =await categoryRepo.HasActiveMedicines(id);
                

            if (hasMedicines)
            {
                throw new Exception("Cannot delete a category that contains active medicines");
                  
               
            }

            await categoryRepo.CategoryDelete(category);

            return true;
        }

        // Converts Category model into CategoryDto
        private CategoryDto ConvertToDto(Category category)
        {
            CategoryDto dto = new CategoryDto
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                Description = category.Description,
                IsActive = category.IsActive
            };

            return dto;
        }
    }
}