using CodeClash.Application.Abstractions.RoomManager;
using Microsoft.AspNetCore.SignalR;

namespace CodeClash.API.Hubs;

/// <summary>
/// SignalR hub responsible for real-time
/// collaborative editing.
///
/// Supports:
/// - Room creation
/// - Room joining
/// - Code synchronization
/// - Language synchronization
/// - Cursor tracking
/// </summary>
public sealed class EditorHub(
    IRoomManager rooms) : Hub
{
    /// <summary>
    /// Creates a room and automatically joins it.
    /// </summary>
    /// <returns>
    /// Newly created room ID.
    /// </returns>
    public async Task<string> CreateRoom(
        string userName)
    {
        var roomId =
            rooms.CreateRoom(
                Context.ConnectionId);

        await JoinRoom(
            userName,
            roomId);

        return roomId;
    }

    /// <summary>
    /// Adds a user into an existing room.
    /// Returns current editor state.
    /// </summary>
    public async Task<object?> JoinRoom(
        string userName,
        string roomId)
    {
        if (!rooms.RoomExists(roomId))
        {
            await Clients.Caller
                .SendAsync(
                    "RoomNotFound",
                    "Room not found");

            return null;
        }

        rooms.AddConnection(
            Context.ConnectionId,
            roomId,
            userName);

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            roomId);

        await Clients.Group(roomId)
            .SendAsync(
                "UserJoined",
                userName);

        var state =
            rooms.GetRoomState(roomId);

        return state is null
            ? null
            : new
            {
                code = state.Value.Code,
                language = state.Value.Language
            };
    }

    /// <summary>
    /// Broadcasts updated editor code.
    /// </summary>
    public async Task SendCode(
        string roomId,
        string code)
    {
        if (!rooms.RoomExists(roomId))
        {
            await Clients.Caller
                .SendAsync(
                    "RoomNotFound",
                    "Room not found");

            return;
        }

        rooms.SetCode(roomId, code);

        await Clients.Group(roomId)
            .SendAsync(
                "ReceiveCode",
                code);
    }

    /// <summary>
    /// Broadcasts language changes.
    /// </summary>
    public async Task SendLanguage(
        string roomId,
        string language)
    {
        if (!rooms.RoomExists(roomId))
        {
            await Clients.Caller
                .SendAsync(
                    "RoomNotFound",
                    "Room not found");

            return;
        }

        rooms.SetLanguage(
            roomId,
            language);

        await Clients
            .OthersInGroup(roomId)
            .SendAsync(
                "ReceiveLanguage",
                language);
    }

    /// <summary>
    /// Updates and broadcasts
    /// cursor position.
    /// </summary>
    public async Task SendCursorPosition(
        string roomId,
        int cursorPosition,
        string color)
    {
        if (!rooms.RoomExists(roomId))
        {
            await Clients.Caller
                .SendAsync(
                    "RoomNotFound",
                    "Room not found");

            return;
        }

        rooms.UpdateCursor(
            Context.ConnectionId,
            cursorPosition,
            color);

        var cursor =
            rooms.GetCursor(
                Context.ConnectionId);

        if (cursor is null)
        {
            return;
        }

        await Clients
            .OthersInGroup(roomId)
            .SendAsync(
                "ReceiveCursorPosition",
                new
                {
                    username =
                        cursor.Value.UserName,

                    index =
                        cursor.Value.Position,

                    color =
                        cursor.Value.Color
                });
    }

    /// <summary>
    /// Executes when a client disconnects.
    /// Removes connection and notifies room.
    /// </summary>
    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        var roomId =
            rooms.GetRoom(
                Context.ConnectionId);

        var userName =
            rooms.GetUserName(
                Context.ConnectionId);

        rooms.RemoveConnection(
            Context.ConnectionId);

        if (roomId is not null)
        {
            await Clients
                .Group(roomId)
                .SendAsync(
                    "UserLeft",
                    userName
                    ?? Context.ConnectionId);
        }

        await base
            .OnDisconnectedAsync(
                exception);
    }
}
