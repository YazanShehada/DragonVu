namespace DragonVu.Application.Features.Profile.Responses;

public sealed record UpdateProfileResult(
    bool Succeeded,
    IReadOnlyList<string> Errors,
    bool IsNotFound = false)
{
    public static UpdateProfileResult Ok() => new(true, []);

    public static UpdateProfileResult NotFound() => new(false, [], true);

    public static UpdateProfileResult Failed(IReadOnlyList<string> errors) => new(false, errors);
}
