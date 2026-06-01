using CodeClash.Domain.Premitives;

namespace CodeClash.Application.EditUserDetails;

public sealed record EditUserDetailsResponse(
    string UserId,
    string? Name,
    string? ImagePath,
    Gender? Gender);
