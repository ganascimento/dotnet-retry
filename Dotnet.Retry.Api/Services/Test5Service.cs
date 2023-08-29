using Dotnet.Retry.Api.Exceptions;

namespace Dotnet.Retry.Api.Services;

public class Test5Service
{
    public async Task<string> Execute1()
    {
        await Task.Delay(2000);
        throw new DomainException() { Code = "095" };
    }

    public async Task<string> Execute2()
    {
        await Task.Delay(2000);
        return "Success 2!!!";
    }
}