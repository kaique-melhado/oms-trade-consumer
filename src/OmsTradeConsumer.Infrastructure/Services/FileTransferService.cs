using OmsTradeConsumer.Domain.Interfaces.Services;
using OmsTradeConsumer.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace OmsTradeConsumer.Infrastructure.FileTransfer;

public class FileTransferService : IFileTransferService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileTransferService> _logger;

    public FileTransferService(IConfiguration configuration, ILogger<FileTransferService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SaveFileAsync(TradeModel tradeModel, string content)
    {
        if (tradeModel == null)
        {
            _logger.LogError("TradeModel é nulo ao tentar salvar arquivo.");
            throw new ArgumentNullException(nameof(tradeModel));
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            _logger.LogError("Conteúdo vazio ou nulo ao tentar salvar arquivo para TradeBlotterId: {TradeBlotterId}", tradeModel.TradeBlotterId);
            throw new ArgumentException("O conteúdo não pode ser nulo ou vazio.", nameof(content));
        }

        try
        {
            var path = _configuration["FileTransfer:Debug"] ?? throw new InvalidOperationException("Configuração do diretório não encontrada em FileTransfer:Debug.");

            if (!Directory.Exists(path))
            {
                _logger.LogInformation("Diretório não encontrado. Criando diretório em: {Path}", path);
                Directory.CreateDirectory(path);
            }

            var fileName = GetFileName(tradeModel);
            var fullPath = Path.Combine(path, fileName);

            _logger.LogInformation("Salvando arquivo em: {FullPath}", fullPath);
            await File.WriteAllTextAsync(fullPath, content, Encoding.UTF8);

            _logger.LogInformation("Arquivo salvo com sucesso para TradeBlotterId: {TradeBlotterId}", tradeModel.TradeBlotterId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar arquivo para TradeBlotterId: {TradeBlotterId}", tradeModel.TradeBlotterId);
            throw;
        }
    }

    private static string GetFileName(TradeModel tradeModel)
    {
        return $"oms-trade-{DateTime.UtcNow:yyyyMMddHHmmss}_{tradeModel.TradeBlotterId}.xml";
    }
}