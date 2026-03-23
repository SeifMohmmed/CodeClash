using CodeClash.Application.DTO;

namespace CodeClash.Application.Abstractions.Identity;
public interface ITokenProvider
{
    AccessTokenDto Create(TokenRequest tokenRequest);
}
