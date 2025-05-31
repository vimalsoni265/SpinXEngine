namespace SpinXEngine.Core.Interface
{
    public interface IBaseService<T>
    {
        /// <summary>
        /// Gets all entities of <typeparamref name="T"/> in the system
        /// </summary>
        /// <returns>A service result containing a list of <typeparamref name="T"/></returns>
        Task<ServiceResult<IEnumerable<T>>> GetAllAsync();
    }
}
