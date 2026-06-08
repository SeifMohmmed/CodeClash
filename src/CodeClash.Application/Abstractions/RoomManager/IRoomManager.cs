namespace CodeClash.Application.Abstractions.RoomManager;

/// <summary>
/// Provides room and connection management
/// for the collaborative editor.
/// </summary>
public interface IRoomManager
{
    /// <summary>
    /// Creates a new room and returns its ID.
    /// </summary>
    string CreateRoom(string connectionId);

    /// <summary>
    /// Checks whether a room exists.
    /// </summary>
    bool RoomExists(string roomId);

    /// <summary>
    /// Adds a user connection to a room.
    /// </summary>
    void AddConnection(
        string connectionId,
        string roomId,
        string userName);

    /// <summary>
    /// Removes a connection from its room.
    /// </summary>
    void RemoveConnection(
        string connectionId);

    /// <summary>
    /// Returns room ID associated with a connection.
    /// </summary>
    string? GetRoom(
        string connectionId);

    /// <summary>
    /// Returns username associated with a connection.
    /// </summary>
    string? GetUserName(
        string connectionId);

    // ====================
    // Editor State
    // ====================

    /// <summary>
    /// Updates room code.
    /// </summary>
    void SetCode(
        string roomId,
        string code);

    /// <summary>
    /// Updates selected programming language.
    /// </summary>
    void SetLanguage(
        string roomId,
        string language);

    /// <summary>
    /// Returns current room state.
    /// </summary>
    (string Code, string Language)? GetRoomState(
        string roomId);

    // ====================
    // Cursor State
    // ====================

    /// <summary>
    /// Updates user cursor information.
    /// </summary>
    void UpdateCursor(
        string connectionId,
        int position,
        string color);

    /// <summary>
    /// Returns current cursor information.
    /// </summary>
    (string UserName, int Position, string Color)?
        GetCursor(string connectionId);
}
