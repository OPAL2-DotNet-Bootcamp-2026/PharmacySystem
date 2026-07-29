using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Category;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService categoryService;

        public CategoryController(CategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        // GET
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Pharmacist")]
        public async Task<IActionResult> GetAllCategories()
        {
            List<CategoryDto> categories =await categoryService.GetAllCategories();
                

            return Ok(categories);
        }

        
        // GET
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Pharmacist")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            CategoryDto? category =await categoryService.GetCategoryById(id);
                

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found"
                });
            }

            return Ok(category);
        }

        // POST
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
            
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
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory( int id,[FromBody] UpdateCategoryDto dto)
           
            
        {
            try
            {
                bool updated = await categoryService.UpdateCategory(id, dto);
                   

                if (!updated)
                {
                    return NotFound(new
                    {
                        message = "Category not found"
                    });
                }

                return Ok(new
                {
                    message = "Category updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                bool deleted =await categoryService.DeleteCategory(id);
                    

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "Category not found"
                    });
                }

                return Ok(new
                {
                    message = "Category deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}