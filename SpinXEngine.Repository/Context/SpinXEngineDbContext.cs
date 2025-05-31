using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SpinXEngine.Common.Helpers;
using SpinXEngine.Repository.Models;
using System.Reflection;

namespace SpinXEngine.Repository.Context
{
    /// <summary>
    /// Represents a database context for interacting with a MongoDB database.
    /// </summary>
    public class SpinXEngineDbContext
    {
        #region Private Members
        private readonly IMongoDatabase m_database;
        private readonly MongoDbSettings m_settings;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public SpinXEngineDbContext(IOptions<MongoDbSettings> options)
        {
            m_settings = options.Value;
            var client = new MongoClient(m_settings.ConnectionString);
            m_database = client.GetDatabase(m_settings.DatabaseName);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves a MongoDB collection for the specified type.
        /// </summary>
        public IMongoCollection<T> GetCollection<T>()
        {
            var attr = typeof(T).GetCustomAttribute<CollectionAttribute>();
            var name = attr?.Name ?? typeof(T).Name.ToLowerInvariant();
            return m_database.GetCollection<T>(name);
        } 

        #endregion
    }
}
