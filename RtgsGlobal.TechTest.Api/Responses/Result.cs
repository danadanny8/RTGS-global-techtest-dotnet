namespace RtgsGlobal.TechTest.Api.Responses;

public record Result
{
	public bool IsSuccess { get; init; }
	public string? ErrorMessage { get; init; }
}
