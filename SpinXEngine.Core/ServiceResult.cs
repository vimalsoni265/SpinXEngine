namespace SpinXEngine.Core
{
    /// <summary>
    /// Represents the result of a service operation, encapsulating the status, optional data, and an optional message.
    /// </summary>
    public class ServiceResult<T>
    {
        #region Properties

        /// <summary>
        /// Gets the current status of the service.
        /// </summary>
        public ServiceStatus Status { get; }

        /// <summary>
        /// Gets the message associated with the current operation or state.
        /// </summary>
        public string? Message { get; }

        /// <summary>
        /// Gets the data associated with the current instance.
        /// </summary>
        public T? Data { get; }

        public bool Success => Status == ServiceStatus.Success; 

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status"></param>
        /// <param name="data"></param>
        /// <param name="message"></param>
        private ServiceResult(ServiceStatus status, T? data = default, string? message = null)
        {
            Status = status;
            Data = data;
            Message = message;
        } 

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Creates a successful service result containing the specified data.
        /// </summary>
        /// <param name="data">The data to include in the service result. This can represent the output of a successful operation.</param>
        public static ServiceResult<T> Ok(T data) =>
            new(ServiceStatus.Success, data);

        /// <summary>
        /// Creates a result indicating that the requested resource was not found.
        /// </summary>
        /// <param name="message">A message describing the reason for the "Not Found" status.</param>
        public static ServiceResult<T> NotFound(string message) =>
            new(ServiceStatus.NotFound, default, message);

        /// <summary>
        /// Creates a service result indicating a validation error.
        /// </summary>
        /// <param name="message">The error message describing the validation issue. Cannot be null or empty.</param>
        public static ServiceResult<T> ValidationError(string message) =>
            new(ServiceStatus.ValidationError, default, message);

        /// <summary>
        /// Creates a result indicating a conflict status with the specified message.
        /// </summary>
        /// <param name="message">The message describing the conflict. Cannot be null or empty.</param>
        public static ServiceResult<T> Conflict(string message) =>
            new(ServiceStatus.Conflict, default, message);

        /// <summary>
        /// Creates a result indicating that the operation was unauthorized.
        /// </summary>
        /// <param name="message">A message describing the unauthorized status. This parameter cannot be null or empty.</param>
        public static ServiceResult<T> Unauthorized(string message) =>
            new(ServiceStatus.Unauthorized, default, message);

        /// <summary>
        /// Creates a result indicating that the requested operation is forbidden.
        /// </summary>
        /// <param name="message">A message describing the reason the operation is forbidden.</param>
        public static ServiceResult<T> Forbidden(string message) =>
            new(ServiceStatus.Forbidden, default, message);

        /// <summary>
        /// Creates a <see cref="ServiceResult{T}"/> instance representing a server error.
        /// </summary>
        /// <param name="message">A descriptive message providing details about the server error.</param>
        public static ServiceResult<T> ServerError(string message) =>
            new(ServiceStatus.ServerError, default, message); 

        #endregion
    }
}
