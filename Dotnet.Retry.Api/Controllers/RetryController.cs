using Dotnet.Retry.Api.Exceptions;
using Dotnet.Retry.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace Dotnet.Retry.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RetryController : ControllerBase
{
    private readonly Test1Service _test1Service;
    private readonly Test2Service _test2Service;
    private readonly Test3Service _test3Service;
    private readonly Test4Service _test4Service;
    private readonly Test5Service _test5Service;

    public RetryController()
    {
        _test1Service = new Test1Service();
        _test2Service = new Test2Service();
        _test3Service = new Test3Service();
        _test4Service = new Test4Service();
        _test5Service = new Test5Service();
    }

    [HttpGet("Success")]
    public async Task<IActionResult> GetRetrySuccess()
    {
        var policy = Policy
            .Handle<Exception>()
            .RetryAsync(3);

        var result = await policy.ExecuteAndCaptureAsync(() => _test1Service.Execute());

        if (result?.Outcome == OutcomeType.Successful)
        {
            return Ok(result.Result);
        }

        var error = result?.FinalException.ToString();
        return BadRequest(error);
    }

    [HttpGet("Error")]
    public async Task<IActionResult> GetRetryError()
    {
        var policy = Policy
            .Handle<InvalidOperationException>()
            .RetryAsync(2);

        Test2Service.RetryCounter = 0;
        var result = await policy.ExecuteAndCaptureAsync(() => _test2Service.Execute());

        if (result?.Outcome == OutcomeType.Successful)
        {
            return Ok(result.Result);
        }

        var error = result?.FinalException.Message;
        return BadRequest(error);
    }

    [HttpGet("Wait")]
    public async Task<IActionResult> GetWait()
    {
        Parallel.Invoke(async () => await Test3Service.CounterAdd());

        var policy = Policy
            .Handle<InvalidValueException>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromSeconds(1 * attempt));

        var result = await policy.ExecuteAndCaptureAsync(() => _test3Service.Execute());

        if (result?.Outcome == OutcomeType.Successful)
        {
            return Ok(result.Result);
        }

        var error = result?.FinalException.Message;
        return BadRequest(error);
    }

    [HttpGet("WaitAtResultTrue")]
    public async Task<IActionResult> GetWaitCustomError()
    {
        Parallel.Invoke(async () => await Test4Service.CounterAdd());

        var policy = Policy
            .Handle<DomainException>(x => x.Code == "110")
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3),
            });

        var result = await policy.ExecuteAndCaptureAsync(() => _test4Service.Execute());

        if (result?.Outcome == OutcomeType.Successful)
        {
            return Ok(result.Result);
        }

        var error = result?.FinalException.Message;
        return BadRequest(error);
    }

    [HttpGet("Fallback")]
    public async Task<IActionResult> GetFallback()
    {
        var policy = Policy<string>
            .Handle<DomainException>(x => x.Code == "095")
            .FallbackAsync<string>((ct) => _test5Service.Execute2());

        var result = await policy.ExecuteAsync(() => _test5Service.Execute1());

        return Ok(result);
    }
}