using CodeClash.Application.Abstractions.File;
using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Application.EditUserDetails;

public sealed class EditUserDetailsQueryHandler(
     IAppDbContext context,
    IFileService fileService) : IQueryHandler<EditUserDetailsQuery, EditUserDetailsResponse>
{
    public async Task<Result<EditUserDetailsResponse>> Handle(
        EditUserDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.IdentityId == request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<EditUserDetailsResponse>(
                new Error("User.NotFound", $"User with ID '{request.UserId}' was not found."));
        }

        if (request.Name is not null)
        {
            user.Name = request.Name;
        }

        if (request.Gender != default)
        {
            user.Gender = request.Gender;
        }

        string? imagePath = null;

        if (request.Image is not null)
        {
            imagePath = await fileService.UploadFileAsync(request.Image, "avatars");

            if (imagePath is "FailedToUploadImage" or "NoImage")
            {
                return Result.Failure<EditUserDetailsResponse>(
                    new Error("File.UploadFailed", "Failed to upload the profile image."));
            }

            user.ImagePath = imagePath;
        }

        user.UpdatedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new EditUserDetailsResponse(
            user.Id,
            user.Name,
            user.ImagePath,
            user.Gender));
    }
}
