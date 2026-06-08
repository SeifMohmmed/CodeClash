using System.Collections.Concurrent;

namespace CodeClash.Infrastructure.Implementation;

/// <summary>
/// Represents a collaborative editor room.
/// Stores current code, language, and connected members.
/// </summary>
internal sealed class Room
{
    /// <summary>
    /// Stores room members using:
    /// Key   → SignalR ConnectionId
    /// Value → Member information
    /// ConcurrentDictionary is used for thread-safe access.
    /// </summary>
    private readonly ConcurrentDictionary<string, Member> _members = new();

    /// <summary>
    /// Current shared code in the room.
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// Current programming language.
    /// </summary>
    public string Language { get; private set; } = "c++";

    /// <summary>
    /// Updates room code.
    /// </summary>
    public void SetCode(string code) => Code = code;

    /// <summary>
    /// Updates selected language.
    /// </summary>
    public void SetLanguage(string language) => Language = language;

    /// <summary>
    /// Adds or updates a room member.
    /// </summary>
    public void AddMember(string connectionId, string userName)
        => _members[connectionId] = new Member(userName);

    /// <summary>
    /// Removes a member from the room.
    /// </summary>
    public bool RemoveMember(string connectionId)
        => _members.TryRemove(connectionId, out _);

    /// <summary>
    /// Returns member data for a connection.
    /// </summary>
    public Member? GetMember(string connectionId)
        => _members.GetValueOrDefault(connectionId);

    /// <summary>
    /// Indicates whether room contains no users.
    /// </summary>
    public bool IsEmpty => _members.IsEmpty;
}

/// <summary>
/// Represents a connected user inside a room.
/// </summary>
internal sealed class Member
{
    public string UserName { get; }   // User display name.

    public int Position { get; set; }  // Current cursor position.

    public string Color { get; set; } = "#ffffff";  // Cursor display color.


    public Member(string userName)
    {
        UserName = userName;
    }
}
