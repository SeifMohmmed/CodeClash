using System.Text.Json.Serialization;

namespace CodeClash.API.Controllers.Contests;

public sealed record UpdateContestRequest(
    string Name,
    string? Description,
   [property: JsonRequired] DateTime StartDate,
    [property: JsonRequired] DateTime EndDate);
