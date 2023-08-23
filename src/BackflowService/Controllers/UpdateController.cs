using Microsoft.AspNetCore.Mvc;

namespace BackflowService.Controllers;

[ApiController]
[Route("[controller]")]
public class UpdateController(ILogger<UpdateController> logger)
    : RequestQueueController(logger, BackflowRequestType.Update)
{
    [HttpPost("{repo}/{sha}", Name = "Update")]
    public Task<IActionResult> Update(string repo, string sha, [FromQuery(Name = "recursive")] bool recursive)
        => CreateInternal(repo, sha, recursive);
}
