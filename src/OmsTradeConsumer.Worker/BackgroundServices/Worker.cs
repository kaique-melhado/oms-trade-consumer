using OmsTradeConsumer.Messaging.Interfaces;

namespace OmsTradeConsumer.Worker.BackgroundServices;

public class Worker : BackgroundService
{
    private readonly IQueueListener _queueListener;
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private int _interval;

    public Worker(IQueueListener queueListener, ILogger<Worker> logger, IConfiguration configuration)
    {
        _queueListener = queueListener ?? throw new ArgumentNullException(nameof(queueListener));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        _interval = _configuration.GetValue<int>("WorkerSettings:ExecutionInterval", 60);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker iniciado em: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Worker processando mensagens em: {time}", DateTimeOffset.Now);

                await _queueListener.ListenForMessagesAsync();

                _logger.LogInformation("Worker concluído com sucesso em: {time}", DateTimeOffset.Now);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("Worker cancelado. Interrupção detectada pelo token.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagens: {message}", ex.Message);
                _logger.LogCritical("O Worker encontrou uma falha crítica que pode impedir seu funcionamento.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_interval), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("Worker cancelado durante a espera de intervalo. Interrupção detectada.");
                break;
            }
        }

        _logger.LogInformation("Worker encerrado em: {time}", DateTimeOffset.Now);
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker está sendo parado em: {time}", DateTimeOffset.Now);

        if (_queueListener is IDisposable disposableListener)
        {
            _logger.LogInformation("Liberando recursos do QueueListener.");
            disposableListener.Dispose();
        }

        await base.StopAsync(stoppingToken);
    }

    public override void Dispose()
    {
        _logger.LogInformation("Liberando recursos do Worker em: {time}", DateTimeOffset.Now);

        if (_queueListener is IDisposable disposableListener)
        {
            disposableListener.Dispose();
        }

        base.Dispose();
    }
}
