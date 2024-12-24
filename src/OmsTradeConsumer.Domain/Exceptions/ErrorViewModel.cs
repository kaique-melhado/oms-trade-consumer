namespace OmsTradeConsumer.Domain.Exceptions;

public class ErrorViewModel
{
    public ErrorViewModel(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
}
