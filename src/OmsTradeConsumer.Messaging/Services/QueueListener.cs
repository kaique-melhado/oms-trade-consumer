using OmsTradeConsumer.Domain.Interfaces.Services;
using OmsTradeConsumer.Domain.Models;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using OmsTradeConsumer.Messaging.Interfaces;
using Microsoft.Extensions.Logging;
using OmsTradeConsumer.Messaging.Configurations;

namespace OmsTradeConsumer.Messaging.Services;

public class QueueListener : IQueueListener, IDisposable
{
    private readonly QueueConfiguration _queueConfiguration;
    private readonly ITradeService _tradeService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<QueueListener> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private bool _disposed;

    public QueueListener(
        QueueConfiguration queueConfiguration,
        IConfiguration configuration,
        ITradeService tradeService,
        ILogger<QueueListener> logger)
    {
        _queueConfiguration = queueConfiguration;
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _tradeService = tradeService ?? throw new ArgumentNullException(nameof(tradeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _connection = _queueConfiguration.Factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public async Task ListenForMessagesAsync()
    {
        var queueName = _configuration["QueueConnection:QueueName"];
        _logger.LogInformation("Iniciando escuta na fila: {QueueName}", queueName);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                _logger.LogInformation("Mensagem recebida: \n{Message}", message);

                await ProcessMessageAsync(message);

                _channel.BasicAck(ea.DeliveryTag, false);
                _logger.LogInformation("Mensagem confirmada: {Message}", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem: \n{Message}.\nMensagem reenviada para a fila.", message);
                _channel.BasicNack(ea.DeliveryTag, false, true); 
            }
        };

        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        _logger.LogInformation("Escuta na fila {QueueName} iniciada.", queueName);

        await Task.CompletedTask;
    }

    public async Task ProcessMessageAsync(string message)
    {
        try
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("Mensagem vazia ou inválida: \n{Message}", message);
                throw new InvalidOperationException("Mensagem não contém transações válidas.");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var trades = JsonSerializer.Deserialize<List<TradeModel>>(message, options);

            _logger.LogInformation("Iniciando processamento de {Count} transações.", trades.Count);

            await Task.WhenAll(trades.Select(trade => _tradeService.ProcessTradeAsync(trade)));

            _logger.LogInformation("Processamento concluído para {Count} transações.", trades.Count);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao desserializar a mensagem: \n{Message}", message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado durante o processamento da mensagem: \n{Message}", message);
            throw;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _channel?.Close();
                _connection?.Close();
                _channel?.Dispose();
                _connection?.Dispose();
                _logger.LogInformation("Recursos do QueueListener foram liberados.");
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~QueueListener()
    {
        Dispose(false);
    }
}