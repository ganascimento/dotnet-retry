namespace Dotnet.Retry.Api.Services;

public class Test1Service
{
    public static short RetryCounter = 0;

    public async Task<string> Execute()
    {
        await Task.Delay(500);

        if (Test1Service.RetryCounter != 2)
        {
            Test1Service.RetryCounter++;
            throw new Exception("Invalid!");
        }

        Test1Service.RetryCounter = 0;

        return "Success!";
    }
}