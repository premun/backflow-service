using Microsoft.AspNetCore.Mvc;

namespace BackflowService.Controllers;

[ApiController]
[Route("[controller]")]
public class InitController(ILogger<InitController> logger)
    : RequestQueueController(logger, BackflowRequestType.Init)
{
    [HttpPost("{repo}/{sha}", Name = "Init")]
    public Task<IActionResult> Init(string repo, string sha, [FromQuery(Name = "recursive")] bool recursive)
        => CreateInternal(repo, sha, recursive);
}
