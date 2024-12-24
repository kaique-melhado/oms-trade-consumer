using System.Runtime.Serialization;

namespace OmsTradeConsumer.Domain.Exceptions.ExceptionTypes;

[Serializable]
public class NoContentException : Exception
{
    public NoContentException()
    {
    }

    public NoContentException(string message) : base(message)
    {
    }

    public NoContentException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public NoContentException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
