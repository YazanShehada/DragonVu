namespace DragonVu.Application.Features.Users.Responses;

public sealed record AssignRoleResult(bool Succeeded, string Message)
{
    public static AssignRoleResult Success(string message) => new(true, message);

    public static AssignRoleResult Failed(string message) => new(false, message);
}
