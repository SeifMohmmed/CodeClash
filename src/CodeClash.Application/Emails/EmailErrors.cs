using CodeClash.Domain.Abstractions;

namespace CodeClash.Application.Emails;
public static class EmailErrors
{
    public static readonly Error SendFailed =
        new("Email.SendFailed", "Error While Sending Email");
}
