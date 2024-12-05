using Microsoft.AspNetCore.Mvc;

namespace Oshop.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        public IActionResult getCategory()
        {
            var category = new List<string>
            {
                "Bread",
                "Dairy",
                "Vegetables",
                "Seasonings",
                "Fruits"
            };

            return Ok(category);
        }
    }
}
