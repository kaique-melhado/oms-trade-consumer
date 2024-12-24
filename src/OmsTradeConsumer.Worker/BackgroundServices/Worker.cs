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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagens: {message}", ex.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(_interval), stoppingToken);
        }

        _logger.LogInformation("Worker encerrado em: {time}", DateTimeOffset.Now);
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker está sendo parado em: {time}", DateTimeOffset.Now);
        await base.StopAsync(stoppingToken);
    }

    public override void Dispose()
    {
        _logger.LogInformation("Liberando recursos do Worker em: {time}", DateTimeOffset.Now);
        base.Dispose();
    }
}
