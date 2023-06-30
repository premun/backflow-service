using Microsoft.AspNetCore.Mvc;

namespace BackflowService.Controllers;

[ApiController]
[Route("[controller]")]
public class BackflowController : ControllerBase
{
    private readonly ILogger<BackflowController> _logger;

    public BackflowController(ILogger<BackflowController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "CreateBackflow")]
    public Task<IActionResult> Create()
    {
        var request = new BackflowRequest
        {
            Id = Guid.NewGuid().ToString()
        };

        _logger.LogInformation($"Creating backflow request {request.Id}");

        RepoInitializer.Requests.Enqueue(request);
        return Task.FromResult<IActionResult>(NoContent());
    }
}
