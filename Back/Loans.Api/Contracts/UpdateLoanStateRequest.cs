using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Loans.Api.Contracts;

public sealed class UpdateLoanStateRequest
{
    [Required]
    public required string Status { get; init; }

    [JsonPropertyName("reason")]
    public string? RejectionReason { get; init; }
}
