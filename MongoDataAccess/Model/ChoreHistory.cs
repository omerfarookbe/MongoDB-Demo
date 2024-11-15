using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDataAccess.Model;

public class ChoreHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public int Id { get; set; }

    public string ChoreId { get; set; }

    public string ChoreText { get; set; }

    public DateTime DateCompleted { get; set; }

    public User WhoCompleted { get; set; }

    public ChoreHistory()
    {

    }

    public ChoreHistory(Chore chore)
    {
        ChoreId = chore.Id;
        ChoreText = chore.ChoreText;
        DateCompleted = chore.LastCompleted ?? DateTime.Now;
        WhoCompleted = chore.AssignedTo;        
    }

}
