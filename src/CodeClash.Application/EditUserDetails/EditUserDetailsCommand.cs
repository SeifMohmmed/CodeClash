using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.EditUserDetails;

public sealed record EditUserDetailsCommand(
    string? Name,
    IFormFile? Image,
    Gender? Gender) : ICommand<EditUserDetailsResponse>;
