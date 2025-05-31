using FluentValidation;
using Microsoft.Extensions.Logging;
using OmsTradeConsumer.Application.Factories;
using OmsTradeConsumer.Domain.Interfaces.Services;
using OmsTradeConsumer.Domain.Models;
using System.Diagnostics;

namespace OmsTradeConsumer.Application.Services;

public class TradeService : ITradeService
{
    private readonly IFileTransferService _fileTransferService;
    private readonly IValidator<TradeModel> _tradeModelValidator;
    private readonly ILogger<TradeService> _logger;

    public TradeService(IFileTransferService fileTransferService, ILogger<TradeService> logger, IValidator<TradeModel> tradeModelValidator)
    {
        _fileTransferService = fileTransferService ?? throw new ArgumentNullException(nameof(fileTransferService));
        _tradeModelValidator = tradeModelValidator ?? throw new ArgumentNullException(nameof(tradeModelValidator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ProcessTradeAsync(TradeModel tradeModel)
    {
        if (tradeModel == null)
        {
            _logger.LogWarning("Nenhuma transação para processar.");
            throw new InvalidOperationException("Mensagem não contém transações válidas.");
        }

        try
        {
            _logger.LogInformation("Transação TradeBlotterId: {TradeBlotterId} iniciou o processamento.", tradeModel.TradeBlotterId);

            ValidateTrade(tradeModel);

            var xmlContent = XmlFactory.CreateXml(tradeModel).ToString();
            await _fileTransferService.SaveFileAsync(tradeModel, xmlContent);

            _logger.LogInformation("Transação TradeBlotterId: {TradeBlotterId} processada com sucesso.", tradeModel.TradeBlotterId);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Falha de validação na transação TradeBlotterId: {TradeBlotterId} \n{Message}", tradeModel.TradeBlotterId, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar a transação TradeBlotterId: {TradeBlotterId} \n{Message}", tradeModel.TradeBlotterId, ex.Message);
            throw;
        }
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
