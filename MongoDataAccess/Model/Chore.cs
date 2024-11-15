using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDataAccess.Model;

public class Chore
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string ChoreText { get; set; }

    public int FrequencyInDays { get; set; }

    public User? AssignedTo { get; set; }

    public DateTime? LastCompleted { get; set; }
}
