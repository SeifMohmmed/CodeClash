using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.EditUserDetails;

public sealed record EditUserDetailsQuery(
    string UserId,
    string? Name,
    IFormFile? Image,
    Gender? Gender) : IQuery<EditUserDetailsResponse>;
