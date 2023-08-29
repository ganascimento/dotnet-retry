using Dotnet.Retry.Api.Exceptions;

namespace Dotnet.Retry.Api.Services;

public class Test3Service
{
    public static short RetryCounter = 0;

    public Task<string> Execute()
    {
        if (Test3Service.RetryCounter < 4)
        {
            Test3Service.RetryCounter++;
            throw new InvalidValueException("Invalid value!");
        }

        Test3Service.RetryCounter = 0;

        return Task.FromResult("Success!");
    }

    public static async Task CounterAdd()
    {
        await Task.Delay(1000);
        Test3Service.RetryCounter++;
        if (Test3Service.RetryCounter != 4) await Test3Service.CounterAdd();
    }
}