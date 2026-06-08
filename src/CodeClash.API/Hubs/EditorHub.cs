using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.Abstractions.RoomManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.API.Hubs;

/// <summary>
/// SignalR hub responsible for real-time collaborative editing.
/// Handles authenticated user sessions and synchronizes editor state
/// between connected clients inside shared rooms.
///
/// Responsibilities:
/// - Create collaborative rooms
/// - Join existing rooms
/// - Synchronize editor code changes
/// - Synchronize programming language changes
/// - Track and broadcast cursor positions
/// - Resolve authenticated user display names
/// - Notify users when participants join or leave
/// </summary>
[Authorize]
public sealed class EditorHub(
    IRoomManager rooms,
    UserManager<IdentityUser> userManager,
    IAppDbContext db) : Hub
{
    /// <summary>
    /// Resolves the display name for the current SignalR connection.
    /// Returns a fallback guest name if the connection
    /// is unauthenticated or the user cannot be resolved.
    /// </summary>
    /// <returns>
    /// User display name for room participation.
    /// </returns>
    private async Task<string> GetCurrentUserName()
    {
        if (Context.User is null)
        {
            return $"Guest-{Random.Shared.Next(1, 100)}";
        }

        var identityUser = await userManager.GetUserAsync(Context.User);
        if (identityUser is null)
        {
            return $"Guest-{Random.Shared.Next(1, 100)}";
        }

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.IdentityId == identityUser.Id);

        return user?.Name ?? identityUser.UserName ?? $"Guest-{Random.Shared.Next(1, 100)}";
    }

    /// <summary>
    /// Creates a room and automatically joins it.
    /// </summary>
    /// <returns>
    /// Newly created room ID.
    /// </returns>
    public async Task<string> CreateRoom()
    {
        var roomId =
            rooms.CreateRoom(
                Context.ConnectionId);

        await JoinRoom(roomId);
        return roomId;
    }

    /// <summary>
    /// Adds a user into an existing room.
    /// Returns current editor state.
    /// </summary>
    public async Task<object?> JoinRoom(
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

        var userName = await GetCurrentUserName();  // <-- server-side resolution

        rooms.AddConnection(
            Context.ConnectionId,
            roomId,
            userName);

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            roomId);

        await Clients.OthersInGroup(roomId)
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

        await Clients.OthersInGroup(roomId)
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
