using CodeClash.Application.Mapping;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CodeClash.Application.Authentication.Register;
internal sealed class RegisterCommandHandler(
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var mappedUser = request.ToApplicationUser();

        var applicationUser = await userManager.FindByEmailAsync(request.Email);

        if (applicationUser is not null)
        {
            return Result.Failure<RegisterResponse>(ApplicationUserErrors.EmailAlreadyExists);
        }

        var applicationUserByUserName = await userManager.FindByNameAsync(request.UserName);

        if (applicationUserByUserName is not null)
        {
            return Result.Failure<RegisterResponse>(ApplicationUserErrors.UserNameAlreadyExists);
        }


        var result = await userManager.CreateAsync(mappedUser, request.Password);

        if (!result.Succeeded)
        {
            return Result.Failure<RegisterResponse>(ApplicationUserErrors.InvalidCredentials);
        }

        var response = new RegisterResponse(
            mappedUser.Id,
            mappedUser.Email!,
            mappedUser.UserName!
        );

        return Result.Success(response, "Registered Successfully");
    }
}
