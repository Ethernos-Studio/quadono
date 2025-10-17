using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Quadono
{
    public class QuadonoHost : IHostedService
    {
        readonly Action _work;
        readonly CancellationTokenSource _cts = new();

        public QuadonoHost(Action work) => _work = work;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                _work();                     // 番茄或别的
                AlarmService.Run(_cts.Token); // 阻塞检查闹钟
            }, cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }
    }
}