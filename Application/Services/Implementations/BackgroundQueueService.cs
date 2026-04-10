using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PAN.API.Domain.Entities;
using PAN.API.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PAN.API.Application.Services.Implementations;

public class BackgroundQueueService : IHostedService
{
    private readonly ConcurrentQueue<PanResponseJson> _queue = new();
    private readonly IServiceScopeFactory _scopeFactory;
    private CancellationTokenSource? _cts;
    private Task? _task;

    public BackgroundQueueService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void Enqueue(PanResponseJson item)
    {
        _queue.Enqueue(item);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _task = Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    if (_queue.TryDequeue(out var item))
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var repo = scope.ServiceProvider.GetRequiredService<IRawResponseRepository>();

                        await repo.InsertAsync(item);
                    }
                    else
                    {
                        await Task.Delay(100, _cts.Token);
                    }
                }
                catch (TaskCanceledException)
                {
                    // Expected during shutdown → ignore
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"BackgroundQueue Error: {ex.Message}");
                }
            }
        }, _cts.Token);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cts != null)
        {
            _cts.Cancel();
        }

        if (_task != null)
        {
            try
            {
                await _task;
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
        }
    }
}