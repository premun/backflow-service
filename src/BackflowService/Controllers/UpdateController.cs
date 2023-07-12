using Microsoft.AspNetCore.Mvc;

namespace BackflowService.Controllers;

[ApiController]
[Route("[controller]")]
public class UpdateController(ILogger<UpdateController> logger)
    : RequestQueueController(logger, BackflowRequestType.Update)
{
    [HttpPost("{repo}/{sha}", Name = "Update")]
    public Task<IActionResult> Create(string repo, string sha) => CreateInternal(repo, sha);
}
