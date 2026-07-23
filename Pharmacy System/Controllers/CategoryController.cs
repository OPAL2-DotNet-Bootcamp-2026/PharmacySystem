using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Category;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService categoryService;

        public CategoryController(CategoryService categoryService)
            
        {
            this.categoryService = categoryService;
        }

        // Returns all active categories
        // GET: api/Category
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            List<CategoryDto> categories = await categoryService.GetAllCategories();
               

            return Ok(categories);
        }

        // Returns one active category by ID
        // GET: api/Category/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            CategoryDto? category = await categoryService.GetCategoryById(id);
               

            if (category == null)
            {
                return NotFound("Category not found");
            }

            return Ok(category);
        }

        // Creates a new category
        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
            
        {
            try
            {
                int categoryId =await categoryService.CreateCategory(dto);
                    

                return Ok(new
                {
                    CategoryID = categoryId,
                    Message = "Category created successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Updates an existing category
        // PUT: api/Category/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id,UpdateCategoryDto dto)
            
            
        {
            try
            {
                bool updated = await categoryService.UpdateCategory(id, dto);
                   

                if (!updated)
                {
                    return NotFound("Category not found");
                }

                return Ok( "Category updated successfully" );
                   
               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        // DELETE: api/Category/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                bool deleted =await categoryService.DeleteCategory(id);

                    
                if (!deleted)
                {
                    return NotFound("Category not found");
                }

                return Ok("Category deleted successfully");
                    
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}