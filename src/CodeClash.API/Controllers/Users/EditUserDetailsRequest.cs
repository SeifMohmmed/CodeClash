using CodeClash.Domain.Premitives;

namespace CodeClash.API.Controllers.Users;

public sealed record EditUserDetailsRequest(
    string? Name,
    IFormFile? Image,
    Gender? Gender);
