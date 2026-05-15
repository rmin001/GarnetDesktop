namespace GarnetDesktop.Core.Models;

public sealed class ServiceResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public string? Output { get; init; }

    public static ServiceResult Ok(string output = "") =>
        new() { Success = true, Output = output };

    public static ServiceResult Fail(string error, string output = "") =>
        new() { Success = false, Error = error, Output = output };
}