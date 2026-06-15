using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Application.Abstractions.File;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.EditUserDetails;

public sealed class EditUserDetailsCommandHandler(
    IUserRepository userRepository,
    IFileService fileService,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : ICommandHandler<EditUserDetailsCommand, EditUserDetailsResponse>
{
    public async Task<Result<EditUserDetailsResponse>> Handle(
        EditUserDetailsCommand request,
        CancellationToken cancellationToken)
    {
        var identityId = currentUserService.IdentityId;

        var user = await userRepository
            .GetByIdentityIdAsync(identityId!);

        if (user is null)
        {
            return Result.Failure<EditUserDetailsResponse>(UserErrors.NotFound);
        }

        if (request.Name is not null)
        {
            user.Name = request.Name;
        }

        if (request.Gender != default)
        {
            user.Gender = request.Gender;
        }


        if (request.Image is not null)
        {
            var imagePath = await fileService
                .UploadFileAsync(request.Image, "avatars");

            if (imagePath is "FailedToUploadImage" or "NoImage")
            {
                return Result.Failure<EditUserDetailsResponse>(
                    new Error("File.UploadFailed", "Failed to upload the profile image."));
            }

            user.ImagePath = imagePath;
        }

        user.UpdatedAtUtc = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new EditUserDetailsResponse(
            user.Id,
            user.Name,
            user.ImagePath,
            user.Gender));
    }
}
