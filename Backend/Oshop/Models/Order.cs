using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Oshop.Models
{
    public class Order
    {
        [BsonId]
        public ObjectId Id { get; set; }  // MongoDB ObjectId
        public long DatePlaced { get; set; } // Date as a Unix timestamp
        public List<product> Items { get; set; } // List of products
        public Shipping Shipping { get; set; } // Shipping details
        public string Username { get; set; } // Username of the user placing the order
    }

    public class product
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
    }

    public class Shipping
    {
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
    }
}
