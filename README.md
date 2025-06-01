# SpinXEngine

SpinXEngine is a slot machine API built in .NET Core API with MongoDB.

## Project Structure

The solution consists of multiple projects:
- **SpinXEngine.Api** - REST API interface for the engine
- **SpinXEngine.Core** - Core business logic implementation
- **SpinXEngine.Repository** - Data access layer for MongoDB
- **SpinXEngine.Common** - Shared utilities and helpers
- **SpinXEngine.Api.Test** - API tests
- **SpinXEngine.TestWinLogic.App** - Test application for game logic

## Setup MongoDB Database

Follow these steps to set up the required MongoDB database:

1. **Install MongoDB** (if not already installed)
   - Download from [MongoDB official website](https://www.mongodb.com/try/download/community)
   - Follow the installation instructions for your platform

2. **Start MongoDB server**
   - On Windows: The MongoDB service should start automatically
   - On Linux/Mac: Run `sudo systemctl start mongod` or `mongod`

3. **Connect to MongoDB**
   - Open MongoDB Compass or use the MongoDB Shell
   - Connect to your MongoDB instance (default: `mongodb://localhost:27017`)

4. **Create Database and Collection**
   - Create a new database named `spinxengine`
   - Create a collection named `players`

5. **Insert Initial Data**
   - Insert the following documents into the `players` collection:
```json
{
  "_id": "player1",
  "balance": { "$numberDecimal": "1000" }
}
{
  "_id": "player2",
  "balance": { "$numberDecimal": "4799.80" }
}
```

## Configuration

Update your application settings in the `appsettings.json` file to include the MongoDB connection string:
```json
{ "ConnectionString": { "MongoDB": "mongodb://localhost:27017/spinxengine" }, "GameSettings": { "ReelRows": 3, "ReelColumns": 5 }}
```

## Running the Application

1. Ensure MongoDB is running
2. Build the solution
3. Run the SpinXEngine.Api project

## Development

The SpinXEngine uses a repository pattern for data access:
- `SpinXEngineDbContext`: Manages database connections
- `IPlayerRepository`: Interface for player data operations
- `PlayerRepository`: Implementation for MongoDB operations

## Testing

Run the test projects to verify the functionality:
- `SpinXEngine.Api.Test`
