using System.Collections.Concurrent;
using System.Diagnostics;
using BackflowService.Controllers;
using Microsoft.DotNet.DarcLib.Helpers;
using Microsoft.DotNet.DarcLib.VirtualMonoRepo;

namespace BackflowService;

public class RequestProcessor : IHostedService
{
    public static readonly ConcurrentQueue<BackflowRequest> Requests = new();
    private readonly IVmrInitializer _vmrInitializer;
    private readonly IVmrUpdater _vmrUpdater;
    private readonly ILogger<RequestProcessor> _logger;
    private readonly string _vmrPath;

    public RequestProcessor(IVmrInitializer vmrInitializer, IVmrUpdater vmrUpdater, ILogger<RequestProcessor> logger, string vmrPath)
    {
        _vmrInitializer = vmrInitializer;
        _vmrUpdater = vmrUpdater;
        _logger = logger;
        _vmrPath = vmrPath;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting " + nameof(RequestProcessor));
        Task.Run(async () => await DoWork(cancellationToken), cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Shutting down {nameof(RequestProcessor)}, deleting {_vmrPath}");
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


            _logger.LogInformation($"Processing backflow request {request.Type} {request.Id}");
            var timer = Stopwatch.StartNew();

            try
            {
                switch (request.Type)
                {
                    case BackflowRequestType.Init:
                        await _vmrInitializer.InitializeRepository(
                            request.Repo,
                            request.Sha,
                            null,
                            false,
                            new UnixPath("/app/source-mappings.json"),
                            Array.Empty<AdditionalRemote>(),
                            readmeTemplatePath: null,
                            tpnTemplatePath: null,
                            cancellationToken);
                        break;
                    case BackflowRequestType.Update:
                        await _vmrUpdater.UpdateRepository(
                            request.Repo,
                            request.Sha,
                            null,
                            true,
                            false,
                            Array.Empty<AdditionalRemote>(),
                            readmeTemplatePath: null,
                            tpnTemplatePath: null,
                            cancellationToken);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(request.Type));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Request {request} failed:{Environment.NewLine}{e}");
                continue;
            }

            _logger.LogInformation($"Processed request {request} in {timer.Elapsed}");
        }

    }
}
