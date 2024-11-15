using MongoDataAccess.Model;
using MongoDB.Driver;
using System.Reflection.Metadata;

namespace MongoDataAccess.DataAccess;

public class ChoreDataAccess
{
    private const string ConnectionString = "mongodb:127.0.0.1:27017";
    string DatabaseName = "chore_db";
    string UserCollection = "users";
    string ChoreCollection = "chore_chart";
    string ChoreHistoryCollection = "chore_history";

    private IMongoCollection<T> ConnectToMongo<T>(in string collection)
    {
        var client = new MongoClient(ConnectionString);
        var db = client.GetDatabase(DatabaseName);
        return db.GetCollection<T>(collection);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        var userCollection = ConnectToMongo<User>(UserCollection);
        var results = await userCollection.FindAsync(_ => true);
        return results.ToList();
    }

    public async Task<List<Chore>> GetAllChorsAsync()
    {
        var choreCollection = ConnectToMongo<Chore>(ChoreCollection);
        var results = await choreCollection.FindAsync(_ => true);
        return results.ToList();
    }

    public async Task<List<Chore>> GetAllChorsForUserAsync(User user)
    {
        var choreCollection = ConnectToMongo<Chore>(ChoreCollection);
        var results = await choreCollection.FindAsync(f => f.AssignedTo.Id == user.Id);
        return results.ToList();
    }

    public async Task<List<ChoreHistory>> GetAllChorHistoryAsync()
    {
        var chorehistoryCollection = ConnectToMongo<ChoreHistory>(ChoreHistoryCollection);
        var results = await chorehistoryCollection.FindAsync(_ => true);
        return results.ToList();
    }

    public async void CreateUserAsync(User user)
    {
        var userCollection = ConnectToMongo<User>(UserCollection);
        await userCollection.InsertOneAsync(user);
    }

    public async void CreateChoreAsync(Chore chore)
    {
        var choreCollection = ConnectToMongo<Chore>(ChoreCollection);
        await choreCollection.InsertOneAsync(chore);
    }

    public async void UpdateChoreAsync(Chore chore)
    {
        var choreCollection = ConnectToMongo<Chore>(ChoreCollection);
        var found = Builders<Chore>.Filter.Eq("Id", chore.Id);
        await choreCollection.ReplaceOneAsync(found, chore, new ReplaceOptions { IsUpsert = true }); // This act as upsert
    }

    public async void DeleteChoreAsync(Chore chore)
    {
        var choreCollection = ConnectToMongo<Chore>(ChoreCollection);
        await choreCollection.DeleteOneAsync<Chore>(f => f.Id == chore.Id);
    }

    public async void CompleteChoreAsync(Chore chore)
    {
        //var choresCollection = ConnectToMongo<Chore>(ChoreCollection);
        //var found = Builders<Chore>.Filter.Eq("Id", chore.Id);
        //await choresCollection.ReplaceOneAsync(found, chore);

        //var choreHistoryCollection = ConnectToMongo<ChoreHistory>(ChoreHistoryCollection);
        //await choreHistoryCollection.InsertOneAsync(new ChoreHistory(chore));

        //MongoDB transaction
        var client = new MongoClient(ConnectionString);
        using var session = await client.StartSessionAsync();
        session.StartTransaction();
        try
        {
            var db = client.GetDatabase(DatabaseName);
            var choresCollection = db.GetCollection<Chore>(ChoreCollection);
            var found = Builders<Chore>.Filter.Eq("Id", chore.Id);
            await choresCollection.ReplaceOneAsync(found, chore);

            var choreHistoryCollection = ConnectToMongo<ChoreHistory>(ChoreHistoryCollection);
            await choreHistoryCollection.InsertOneAsync(new ChoreHistory(chore));

            await session.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(ex.Message.ToString());
        }

    }
}
