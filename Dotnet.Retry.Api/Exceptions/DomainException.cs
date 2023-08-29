namespace Dotnet.Retry.Api.Exceptions;

public class DomainException : Exception
{
    public required string Code { get; set; }
}