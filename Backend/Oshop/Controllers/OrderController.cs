using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Oshop.Models;

namespace Oshop.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class OrderController: ControllerBase
    {
        private readonly IMongoCollection<Order> _dbOrder;

        public OrderController()
        {
            try
            {
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("Oshop");
                _dbOrder = database.GetCollection<Order>("Orders");
                Console.WriteLine("MongoDB connection established successfully in Order Controller");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Order Controller failed to connect to DB: "+ex.Message);
            }
        }

        [HttpPost]
        public IActionResult SaveOrder([FromBody] Order order)
        {
            if(order == null)
            {
                return BadRequest("Invalid order data.");
            }

            try
            {
                _dbOrder.InsertOne(order);
                return Ok(new { message = "Order saved successfully"});
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error saving order: " + ex.Message);
                return StatusCode(500, new { message = "An error occured while saving the order" });
            }
        }

        [HttpGet] 
        public IActionResult GetOrders([FromQuery]string username)
        {
            try
            {
                List<Order> orders;

                if(username == "^_^")
                {
                    orders = (_dbOrder.Find(_ => true).ToList());
                }
                else
                {
                    orders = _dbOrder.Find(itr => itr.Username == username).ToList();
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting order: " + ex.Message);
                return StatusCode(500, new { message = "An error occured while getting the orders" });
            }
        }
    }
}
