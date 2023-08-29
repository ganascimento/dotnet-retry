using Dotnet.Retry.Api.Exceptions;

namespace Dotnet.Retry.Api.Services;

public class Test4Service
{
    public static short RetryCounter = 0;

    public Task<string> Execute()
    {
        if (Test4Service.RetryCounter < 6)
        {
            Test4Service.RetryCounter++;
            throw new DomainException() { Code = "110" };
        }

        Test4Service.RetryCounter = 0;

        return Task.FromResult("Success!");
    }

    public static async Task CounterAdd()
    {
        await Task.Delay(1000);
        Test4Service.RetryCounter++;
        if (Test4Service.RetryCounter < 6) await Test4Service.CounterAdd();
    }
}