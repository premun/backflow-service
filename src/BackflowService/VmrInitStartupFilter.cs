using LibGit2Sharp;
using Microsoft.DotNet.DarcLib;

namespace BackflowService;

public class VmrInitStartupFilter(ILogger<VmrInitStartupFilter> logger, string vmrPath) : IStartupFilter
{
    private readonly ILogger<VmrInitStartupFilter> _logger = logger;
    private readonly string _vmrPath = vmrPath;

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => builder =>
    {
        _logger.LogInformation($"Initializing VMR at {_vmrPath}...");

        Directory.CreateDirectory(_vmrPath);
        using var repo = new Repository(Repository.Init(_vmrPath));
        var signature = new Signature(Constants.DarcBotName, Constants.DarcBotEmail, DateTimeOffset.Now);
        repo.Commit("Initial commit", signature, signature, new CommitOptions { AllowEmptyCommit = true });

        next(builder);
    };
}
