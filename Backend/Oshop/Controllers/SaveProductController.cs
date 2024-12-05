using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using Oshop.Models;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authorization;

namespace Oshop.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SaveProductController : ControllerBase
    {
        private readonly IMongoCollection<Product> dbProduct;

        public SaveProductController()
        {
            try
            {
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("Oshop");
                dbProduct = database.GetCollection<Product>("product");
                Console.WriteLine("MongoDB connection established successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect to MongoDB: " + ex.Message);
            }
        }

        public string print()
        {
            return "Running";
        }

        [Authorize]
        [HttpPost]
        public IActionResult SaveProduct([FromBody] Product value)
        {
            if (value == null)
            {
                return BadRequest("Invalid product data.");
            }

            try
            {
                dbProduct.InsertOne(value);
                return Ok(new { message = "Product saved successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving product: " + ex.Message);
                return StatusCode(500, new { message = "An error occurred while saving the product." });
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetProduct()
        {
            try
            {
                // Retrieving all products from the database
                var products = dbProduct.Find(_ => true).ToList();

                // Returning the products in the response
                return Ok(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving products: " + ex.Message);
                return StatusCode(500, new { message = "An error occurred while retrieving the products." });
            }
        }

        [HttpGet]
        public IActionResult GetWithId(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out ObjectId objectId))
                {
                    return BadRequest("Invalid product ID format.");
                }

                // Create a filter to find the product by ObjectId
                var filter = Builders<Product>.Filter.Eq("_id", objectId);

                // Find product by Id using the filter
                var product = dbProduct.Find(filter).FirstOrDefault();

                if (product == null)
                {
                    return NotFound(new { message = "Product not found." });
                }

                // Return the found product
                return Ok(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving product: " + ex.Message);
                return StatusCode(500, new { message = "An error occurred while retrieving the product." });
            }
        }

        [HttpPut]
        public IActionResult Update([FromBody] Product product)
        {
            try
            {
                //Check if the product ID is valid
                if (!ObjectId.TryParse(product.Id, out ObjectId objectId))
                {
                    return BadRequest("Invalid product ID format.");
                }

                // Check if the provided product object is valid
                if (product == null)
                {
                    return BadRequest("Invalid product data.");
                }

                // Create a filter to find the product by ID
                var filter = Builders<Product>.Filter.Eq("_id", objectId);

                // Check if the product exists in the database
                var existingProduct = dbProduct.Find(filter).FirstOrDefault();
                if (existingProduct == null)
                {
                    return NotFound(new { message = "Product not found." });
                }

                // Create the update definition to update the product's fields
                var update = Builders<Product>.Update
                    .Set(p => p.Title, product.Title)
                    .Set(p => p.Price, product.Price)
                    .Set(p => p.Category, product.Category)
                    .Set(p => p.ImageUrl, product.ImageUrl);

                // Perform the update operation
                var result = dbProduct.UpdateOne(filter, update);

                // Check if the update was successful
                if (result.ModifiedCount == 0)
                {
                    return StatusCode(500, new { message = "Failed to update the product. No changes detected." });
                }

                // Return a success message
                return Ok(new { message = "Product updated successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating product: " + ex.Message);
                return StatusCode(500, new { message = "An error occurred while updating the product." });
            }
        }

        [HttpDelete]
        public IActionResult Delete(string Id)
        {
            try
            {
                // Validate if the provided Id can be converted to an ObjectId
                if (!ObjectId.TryParse(Id, out ObjectId objectId))
                {
                    return BadRequest("Invalid product ID format.");
                }

                // Create a filter to match the product by Id
                var filter = Builders<Product>.Filter.Eq("_id", objectId);

                // Attempt to delete the product
                var result = dbProduct.DeleteOne(filter);

                // Check if a product was deleted
                if (result.DeletedCount == 0)
                {
                    return NotFound(new { message = "Product not found." });
                }

                // Return a success message
                return Ok(new { message = "Product deleted successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting product: " + ex.Message);
                return StatusCode(500, new { message = "An error occurred while deleting the product." });
            }
        }
    }
}
