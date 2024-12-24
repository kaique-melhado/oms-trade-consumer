using FluentValidation;
using Microsoft.Extensions.Logging;
using OmsTradeConsumer.Application.Factories;
using OmsTradeConsumer.Domain.Interfaces.Services;
using OmsTradeConsumer.Domain.Models;

namespace OmsTradeConsumer.Application.Services;

public class TradeService : ITradeService
{
    private readonly IFileTransferService _fileTransferService;
    private readonly ILogger<TradeService> _logger;
    private readonly IValidator<TradeModel> _tradeModelValidator;

    public TradeService(IFileTransferService fileTransferService, ILogger<TradeService> logger, IValidator<TradeModel> tradeModelValidator)
    {
        _fileTransferService = fileTransferService ?? throw new ArgumentNullException(nameof(fileTransferService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tradeModelValidator = tradeModelValidator ?? throw new ArgumentNullException(nameof(tradeModelValidator));
    }

    public async Task ProcessTradesAsync(List<TradeModel> tradesModel)
    {
        if (tradesModel == null || !tradesModel.Any())
        {
            _logger.LogWarning("Nenhuma transação para processar.");
            return;
        }

        _logger.LogInformation("Iniciando processamento de {Count} transações.", tradesModel.Count);

        foreach (var trade in tradesModel)
        {
            try
            {
                ValidateTrade(trade);

                var xmlContent = XmlFactory.CreateXml(trade).ToString();
                await _fileTransferService.SaveFileAsync(trade, xmlContent);

                _logger.LogInformation("Transação TradeBlotterId = {TradeBlotterId} processada com sucesso.", trade.TradeBlotterId);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Falha de validação na transação TradeBlotterId = {TradeBlotterId}: {Message}", trade.TradeBlotterId, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar a transação TradeBlotterId = {TradeBlotterId}: {Message}", trade.TradeBlotterId, ex.Message);
                throw;
            }
        }

        _logger.LogInformation("Processamento concluído para {Count} transações.", tradesModel.Count);
    }

    private void ValidateTrade(TradeModel trade)
    {
        var validationResult = _tradeModelValidator.Validate(trade);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(errors);
        }
    }
}
