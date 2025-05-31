using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SpinXEngine.Common.Helpers;
using SpinXEngine.Repository.Models;
using System.Reflection;

namespace SpinXEngine.Repository.Context
{
    public class SpinXEngineDbContext
    {
        private readonly IMongoDatabase m_database;
        private readonly MongoDbSettings m_settings;

        public SpinXEngineDbContext(IOptions<MongoDbSettings> options)
        {
            m_settings = options.Value;
            var client = new MongoClient(m_settings.ConnectionString);
            m_database = client.GetDatabase(m_settings.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            var attr = typeof(T).GetCustomAttribute<CollectionAttribute>();
            var name = attr?.Name ?? typeof(T).Name.ToLowerInvariant();
            return m_database.GetCollection<T>(name);
        }
    }
}
