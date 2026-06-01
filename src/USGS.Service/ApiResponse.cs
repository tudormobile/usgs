namespace Tudormobile.USGS.Service;

/// <summary>
/// Represents the result of an API call, containing a success flag and optional data.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed record ApiResponse<T>
{
    /// <summary>
    /// Gets a value indicating whether the API call was successful.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Gets the data returned by the API call, can be <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// May contain error information on failure. Can be null on success when no data is returned.
    /// </remarks>
    public T? Data { get; init; }
}

internal static class ApiResponse
{
    /// <summary>
    /// Creates a successful API response containing the specified value.
    /// </summary>
    /// <param name="value">The value to include in the response data.</param>
    /// <returns>A new <see cref="ApiResponse{T}"/> instance with the specified value set as data and the success flag set to true.</returns>
    public static ApiResponse<T> Success<T>(T value) => new() { Data = value, IsSuccess = true };
    /// <summary>
    /// Creates a failed response containing the specified value.
    /// </summary>
    /// <param name="value">The value to include in the response's data payload.</param>
    /// <returns>A new <see cref="ApiResponse{T}"/> instance with Success set to false and Data set to the specified value.</returns>
    public static ApiResponse<T> Failure<T>(T value) => new() { Data = value, IsSuccess = false };
}
