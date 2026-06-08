using System.Collections.Concurrent;
using CodeClash.Application.Abstractions.RoomManager;

namespace CodeClash.Infrastructure.Implementation;

/// <summary>
/// Manages room lifecycle and user connections.
/// Responsible for:
/// - Creating rooms
/// - Tracking connections
/// - Synchronizing editor state
/// - Managing cursor updates
/// </summary>
internal sealed class RoomManager : IRoomManager
{
    /// <summary>
    /// Maps:
    /// ConnectionId → RoomId
    /// </summary>
    private readonly ConcurrentDictionary<string, string>
        _connections = new();

    /// <summary>
    /// Maps:
    /// RoomId → Room
    /// </summary>
    private readonly ConcurrentDictionary<string, Room>
        _rooms = new();

    /// <summary>
    /// Adds a user connection to a room.
    /// </summary>
    public void AddConnection(
        string connectionId,
        string roomId,
        string userName)
    {
        if (!_rooms.TryGetValue(roomId, out var room))
        {
            return;
        }

        room.AddMember(connectionId, userName);

        // Store reverse lookup
        _connections[connectionId] = roomId;
    }

    /// <summary>
    /// Creates a unique room ID.
    /// Example: A1B2C3
    /// </summary>
    public string CreateRoom(string connectionId)
    {
        string roomId;

        do
        {
            roomId = Guid.NewGuid()
                .ToString("N")[..6]
                .ToUpper();
        }
        while (!_rooms.TryAdd(roomId, new Room()));

        return roomId;
    }

    /// <summary>
    /// Returns username by connection.
    /// </summary>
    public string? GetUserName(string connectionId)
    {
        if (!_connections.TryGetValue(connectionId, out var roomId))
        {
            return null;
        }

        return _rooms.TryGetValue(roomId, out var room)
            ? room.GetMember(connectionId)?.UserName
            : null;
    }

    /// <summary>
    /// Updates shared code.
    /// </summary>
    public void SetCode(string roomId, string code)
    {
        if (_rooms.TryGetValue(roomId, out var room))
        {
            room.SetCode(code);
        }
    }

    /// <summary>
    /// Updates selected language.
    /// </summary>
    public void SetLanguage(string roomId, string language)
    {
        if (_rooms.TryGetValue(roomId, out var room))
        {
            room.SetLanguage(language);
        }
    }

    /// <summary>
    /// Gets current room editor state.
    /// </summary>
    public (string Code, string Language)? GetRoomState(
        string roomId)
        => _rooms.TryGetValue(roomId, out var room)
            ? (room.Code, room.Language)
            : null;

    /// <summary>
    /// Updates cursor position and color.
    /// </summary>
    public void UpdateCursor(
        string connectionId,
        int position,
        string color)
    {
        if (!_connections.TryGetValue(connectionId, out var roomId))
        {
            return;
        }

        if (!_rooms.TryGetValue(roomId, out var room))
        {
            return;
        }

        var member = room.GetMember(connectionId);

        if (member is null)
        {
            return;
        }

        member.Position = position;
        member.Color = color;
    }

    /// <summary>
    /// Gets current cursor state.
    /// </summary>
    public (string UserName, int Position, string Color)?
        GetCursor(string connectionId)
    {
        if (!_connections.TryGetValue(connectionId, out var roomId))
        {
            return null;
        }

        if (!_rooms.TryGetValue(roomId, out var room))
        {
            return null;
        }

        var member = room.GetMember(connectionId);

        return member is null
            ? null
            : (member.UserName,
               member.Position,
               member.Color);
    }

    /// <summary>
    /// Returns room ID for a connection.
    /// </summary>
    public string? GetRoom(string connectionId)
        => _connections.GetValueOrDefault(connectionId);

    /// <summary>
    /// Removes user connection.
    /// Deletes room if empty.
    /// </summary>
    public void RemoveConnection(string connectionId)
    {
        if (!_connections.TryRemove(
            connectionId,
            out var roomId))
        {
            return;
        }

        if (!_rooms.TryGetValue(roomId, out var room))
        {
            return;
        }

        room.RemoveMember(connectionId);

        // Cleanup empty rooms
        if (room.IsEmpty)
        {
            _rooms.TryRemove(roomId, out _);
        }
    }

    /// <summary>
    /// Checks if room exists.
    /// </summary>
    public bool RoomExists(string roomId)
        => _rooms.ContainsKey(roomId);
}
