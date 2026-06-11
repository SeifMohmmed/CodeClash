using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace CodeClash.API.Hubs;

/// <summary>
/// SignalR hub responsible for handling real-time video call communication.
///
/// Supports:
/// - Joining and leaving call rooms
/// - WebRTC offer exchange
/// - WebRTC answer exchange
/// - ICE candidate signaling
/// - Broadcasting participant status updates
/// </summary>
public sealed class VideoChatHub : Hub
{
    /// <summary>
    /// Gets the current user's display name from claims.
    /// Falls back to a generated guest name if the user is anonymous.
    /// </summary>
    private string GetCurrentUserName()
        => Context.User?.FindFirstValue(ClaimTypes.Name)
        ?? $"Guest-{Random.Shared.Next(1, 100)}";

    /// <summary>
    /// Adds the current connection to a video call room
    /// and notifies all participants that a user joined.
    /// </summary>
    public async Task JoinCall(string roomId)
    {
        var userName = GetCurrentUserName();

        // Add current connection to the SignalR group
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            roomId);

        // Notify room participants about the new user
        await Clients
            .Group(roomId)
            .SendAsync(
            "ReceiveMessage",
            $"{userName} has joined the call.");
    }

    /// <summary>
    /// Removes the current connection from a video call room
    /// and notifies all participants that a user left.
    /// </summary>
    public async Task LeaveCall(string roomId)
    {
        var userName = GetCurrentUserName();

        // Remove current connection from the SignalR group
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            roomId);

        // Notify room participants that the user left
        await Clients
            .Group(roomId)
            .SendAsync(
                "ReceiveMessage",
                 $"{userName} has left the call.");
    }

    /// <summary>
    /// Sends a WebRTC offer from the current user
    /// to a target participant.
    /// </summary>
    public async Task SendOffer(
        string offer,
        string targetUser)
        => await Clients.Client(targetUser).SendAsync("ReceiveOffer", Context.ConnectionId, offer);

    /// <summary>
    /// Sends a WebRTC answer from the current user
    /// to a target participant.
    /// </summary>
    public async Task SendAnswer(
        string answer,
        string targetUser)
        => await Clients.Client(targetUser).SendAsync("ReceiveAnswer", Context.ConnectionId, answer);

    /// <summary>
    /// Sends an ICE candidate to a target participant
    /// for establishing peer-to-peer connectivity.
    /// </summary>
    public async Task SendIceCandidate(
        string candidate,
        string targetUser)
        => await Clients.Client(targetUser).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
}
