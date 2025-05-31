namespace SpinXEngine.Core
{
    /// <summary>
    /// Represents the status of a service operation.
    /// </summary>
    public enum ServiceStatus
    {
        Success,
        NotFound,
        ValidationError,
        Conflict,
        Unauthorized,
        Forbidden,
        ServerError,
    }
}
