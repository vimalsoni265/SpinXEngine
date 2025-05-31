using System.Linq.Expressions;

namespace SpinXEngine.Repository.Interfaces
{
    /// <summary>
    /// Interface for a generic repository pattern to handle DataBase operations.
    /// </summary>
    /// <typeparam name="T">The Repository Model class for which Database operations to be carried out</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Function to get all records of type <typeparamref name="T"/> asynchronously.
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Function to Get Record by Id of type <typeparamref name="T"/> asynchronously.
        /// </summary>
        /// <param name="id">The Id based on which record needs to be searched</param>
        /// <returns>Record from the table</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Function to Get Record by Id of type <typeparamref name="T"/> asynchronously.
        /// </summary>
        /// <param name="id">The Id based on which record needs to be searched</param>
        /// <returns>Record from the table</returns>
        Task<T> GetByIdAsync(string id);

        /// <summary>
        /// Function to Insert Record in table of type <typeparamref name="T"/> asynchronously.
        /// </summary>
        /// <param name="entity">Record to be updated</param>
        /// <returns>Record from the table</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Function to Update Record in table of type <typeparamref name="T"/> asynchronously.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Record from the table</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Function to Delete Record from table of type <typeparamref name="T"/> asynchronously.
        /// </summary>
        /// <param name="entity">Record to be deleted</param>
        /// <returns>Record from the table</returns>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Function to Find List of Record from the table of type <typeparamref name="T"/> asynchronously.
        /// </summary>
        /// <param name="predicate">Delegate to Find the Record</param>
        /// <returns>List of Record from the table</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}
