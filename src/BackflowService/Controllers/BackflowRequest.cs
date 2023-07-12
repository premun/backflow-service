namespace BackflowService.Controllers;

public record BackflowRequest
{
    public required string Id { get; init; }

    public required BackflowRequestType Type { get; init; }

    public required string Repo { get; init; }

    public required string Sha { get; init; }
}
