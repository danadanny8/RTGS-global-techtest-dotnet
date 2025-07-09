namespace RtgsGlobal.TechTest.Api.Responses;

public record AccountApiError
{
	public string Title { get; init; } = "Account API Error";
	public string Message { get; init; }
}
