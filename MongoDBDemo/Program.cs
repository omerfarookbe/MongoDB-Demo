using MongoDB.Driver;
using MongoDBDemo;
using MongoDataAccess.DataAccess;
using MongoDataAccess.Model;

ChoreDataAccess db = new ChoreDataAccess();
db.CreateUserAsync(new User() { FirstName = "Omer", LastName = "Mohideen" });

var user = await db.GetAllUsersAsync();

var chore = new Chore
{
    AssignedTo = user.First(),
    ChoreText = "Clean the house",
    FrequencyInDays = 7
};

db.CreateChoreAsync(chore);

db.UpdateChoreAsync(chore);