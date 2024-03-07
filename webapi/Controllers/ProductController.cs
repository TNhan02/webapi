using Microsoft.AspNetCore.Mvc;
using webapi.Models.DTO;
using webapi.Services;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProduct _productService;

        public ProductController(IProduct productService)
        {
            _productService = productService;
        }

        [HttpGet(Name = "GetAllProducts")]
        public async Task<IActionResult> GetProducts([FromQuery] PaginationFilter filter)
        {
            if(User.HasClaim("Role", "Admin"))
            {
                var products = await _productService.GetProducts(filter);
                return Json(products);
            }
            else
            {
                return BadRequest("Admin role is required! Please check your token again!");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProduct(id);
            return Json(product);
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] DTOProduct newProduct)
        {
            var createdProduct = await _productService.PostProduct(newProduct);
            return Json(createdProduct);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProduct(int id, [FromBody] DTOProduct editUser)
        {
            var editedProduct = await _productService.PatchProduct(id, editUser);
            return Json(editedProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deletedProduct = await _productService.DeleteProduct(id);
            return NoContent();
        }
    }
}
