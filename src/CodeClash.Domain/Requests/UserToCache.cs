namespace CodeClash.Domain.Requests;

public sealed class UserToCache
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string? UserImage { get; set; }
}
