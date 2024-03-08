using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatroomB_Backend.Models
{
    public class ErrorHandle
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string ErrorMessage { get; set; } = null!;

        public string ControllerName { get; set; } = null!;

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Timestamp { get; set;}
    }
}
