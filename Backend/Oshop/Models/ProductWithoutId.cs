using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Oshop.Models
{
    public class ProductWithoutId
    {
        [BsonId]  // Marks this as the primary key in MongoDB
        [BsonRepresentation(BsonType.ObjectId)] // Serializes ObjectId as a string

        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
    }
}
