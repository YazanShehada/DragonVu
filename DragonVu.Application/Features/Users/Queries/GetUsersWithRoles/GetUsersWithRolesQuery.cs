using DragonVu.Application.Features.Users.Responses;
using LiteBus.Queries.Abstractions;

namespace DragonVu.Application.Features.Users.Queries.GetUsersWithRoles;

public sealed record GetUsersWithRolesQuery : IQuery<IReadOnlyList<UserWithRoleResponse>>;
