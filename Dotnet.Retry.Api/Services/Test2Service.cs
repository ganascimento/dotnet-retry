namespace Dotnet.Retry.Api.Services;

public class Test2Service
{
    public static short RetryCounter = 0;

    public async Task<string> Execute()
    {
        await Task.Delay(500);

        if (Test2Service.RetryCounter != 3)
        {
            Test2Service.RetryCounter++;
            throw new InvalidOperationException("Invalid operation!");
        }

        Test2Service.RetryCounter = 0;

        return "Success!";
    }
}