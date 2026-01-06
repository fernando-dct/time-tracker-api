namespace TimeTracker.Api.Common.Exceptions;

public class BusinessException(string message) : Exception(message)
{
}