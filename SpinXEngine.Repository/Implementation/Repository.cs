using MongoDB.Driver;
using SpinXEngine.Repository.Context;
using SpinXEngine.Repository.Interfaces;
using System.Linq.Expressions;

namespace SpinXEngine.Repository.Implementation
{
    /// <summary>
    /// Class implements <seealso cref="IRepository{T}"/> for database operations
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        #region Private Members
        protected readonly SpinXEngineDbContext m_dbContext;
        protected readonly IMongoCollection<T> m_collection;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public Repository(SpinXEngineDbContext context)
        {
            m_dbContext = context;
            m_collection = m_dbContext.GetCollection<T>();
        } 

        #endregion

        #region Implement IRepository
        public virtual async Task<T> AddAsync(T entity)
        {
            await m_collection.InsertOneAsync(entity);
            return entity;
        }

        public virtual async Task DeleteAsync(T entity)
        {
            var idProperty = entity.GetType().GetProperty("Id")
                ?? throw new ArgumentException("Entity does not have an Id property.");

            var filter = Builders<T>.Filter.Eq("Id", idProperty);
            await m_collection.DeleteOneAsync(filter);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await m_collection.Find(predicate).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await m_collection.Find(Builders<T>.Filter.Empty).ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await GetByIdAsync(id.ToString());
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            return await m_collection.Find(filter).FirstOrDefaultAsync();
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            var idProperty = entity.GetType().GetProperty("Id")
                ?? throw new ArgumentException("Entity does not have an Id property.");

            var filter = Builders<T>.Filter.Eq("Id", idProperty);
            await m_collection.ReplaceOneAsync(filter, entity);
            return entity;
        } 

        #endregion
    }
}
