using System.Runtime.Serialization;

namespace OmsTradeConsumer.Domain.Exceptions.ExceptionTypes;

[Serializable]
public class UnprocessableEntityException : Exception
{
    public UnprocessableEntityException()
    {
    }

    public UnprocessableEntityException(string message) : base(message)
    {
    }

    public UnprocessableEntityException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public UnprocessableEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
