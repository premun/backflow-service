using Microsoft.AspNetCore.Mvc;

namespace BackflowService.Controllers;

[ApiController]
[Route("[controller]")]
public class InitController(ILogger<InitController> logger)
    : RequestQueueController(logger, BackflowRequestType.Init)
{
    [HttpPost("{repo}/{sha}", Name = "Init")]
    public Task<IActionResult> Create(string repo, string sha) => CreateInternal(repo, sha);
}
