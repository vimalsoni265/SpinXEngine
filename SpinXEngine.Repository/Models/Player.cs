using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace SpinXEngine.Repository.Models
{
    [Collection("players")]
    public class Player
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("balance")]
        [JsonPropertyName("balance")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Balance { get; set; }
    }
}
