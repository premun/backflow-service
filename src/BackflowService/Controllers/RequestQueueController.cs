using Microsoft.AspNetCore.Mvc;

namespace BackflowService.Controllers;

public abstract class RequestQueueController : ControllerBase
{
    private readonly ILogger<RequestQueueController> _logger;
    private readonly BackflowRequestType _type;

    protected RequestQueueController(ILogger<RequestQueueController> logger, BackflowRequestType type)
    {
        _logger = logger;
        _type = type;
    }

    protected Task<IActionResult> CreateInternal(string repo, string sha, bool recursive)
    {
        var request = new BackflowRequest
        {
            Id = Guid.NewGuid().ToString(),
            Type = _type,
            Repo = repo,
            Sha = sha,
            Recursive = recursive,
        };

        _logger.LogInformation($"Creating {_type} backflow request {request.Id}");

        RequestProcessor.Requests.Enqueue(request);
        return Task.FromResult<IActionResult>(NoContent());
    }
}
