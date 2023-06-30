using System.Collections.Concurrent;
using System.Diagnostics;
using BackflowService.Controllers;
using Microsoft.DotNet.DarcLib.Helpers;
using Microsoft.DotNet.DarcLib.VirtualMonoRepo;

namespace BackflowService;

public class RepoInitializer : IHostedService
{
    public static readonly ConcurrentQueue<BackflowRequest> Requests = new();
    private readonly IVmrInitializer _vmrInitializer;
    private readonly ILogger<RepoInitializer> _logger;
    private readonly string _vmrPath;

    public RepoInitializer(IVmrInitializer vmrInitializer, ILogger<RepoInitializer> logger, string vmrPath)
    {
        _vmrInitializer = vmrInitializer;
        _logger = logger;
        _vmrPath = vmrPath;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting " + nameof(RepoInitializer));
        Task.Run(async () => await DoWork(cancellationToken), cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Shutting down {nameof(RepoInitializer)}, deleting {_vmrPath}");
        Directory.Delete(_vmrPath);

        return Task.CompletedTask;
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (!Requests.TryDequeue(out var request))
            {
                await Task.Delay(5000, cancellationToken);
                continue;
            }


            _logger.LogInformation($"Processing backflow request {request}");
            var timer = Stopwatch.StartNew();

            try
            {
                await _vmrInitializer.InitializeRepository(
                    "installer",
                    null,
                    null,
                    true,
                    new UnixPath("/app/source-mappings.json"),
                    Array.Empty<AdditionalRemote>(),
                    readmeTemplatePath: null,
                    tpnTemplatePath: null,
                    cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Request {request} failed:{Environment.NewLine}{e}");
                continue;
            }

            _logger.LogInformation($"Processed request {request} in {timer.Elapsed}");
        }

    }
}
