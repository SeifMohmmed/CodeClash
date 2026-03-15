using CodeClash.Application.Mapping;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CodeClash.Application.Authentication.Login;
internal sealed class LoginQueryHandler(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager)
    : IRequestHandler<LoginQuery, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(
        LoginQuery request,
        CancellationToken cancellationToken)
    {
        var mappedUser = request.ToApplicationUser();

        var appUser = await userManager.FindByEmailAsync(request.Email);

        if (appUser is null)
        {
            return Result.Failure<LoginResponse>(ApplicationUserErrors.NotFound);
        }

        var result =
            await signInManager.CheckPasswordSignInAsync(appUser, request.Password, false);

        if (!result.Succeeded)
        {
            return Result.Failure<LoginResponse>(ApplicationUserErrors.InvalidCredentials);
        }

        var response = new LoginResponse(
                mappedUser.Id,
                mappedUser.Email!,
                mappedUser.UserName!);


        return Result.Success(response, "Login Successfully!");
    }
}
