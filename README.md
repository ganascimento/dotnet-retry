# Dotnet Retry

This project was developed to test resilience and transient-fault-handling in dotnet applications.

## Resources used

To develop this project, was used:

- DotNet 7
- Polly

## Whats is resilience and transient-fault-handling?

Transient fault handling pertains to managing temporary and self-correcting issues that can disrupt normal operations. Strategies include implementing retries, exponential backoffs, timeouts, and fallbacks. Automatic retries with increasing delays allow systems to recover from transient faults autonomously, while timeouts prevent indefinite resource consumption. Fallback mechanisms ensure users receive some level of service even when primary components falter temporarily.

In essence, resilience focuses on a system's ability to adapt and recover from failures, shielding against catastrophic collapse, while transient fault handling addresses short-lived disruptions, maintaining consistent service availability. These concepts collectively empower APIs to continue functioning effectively even when faced with unexpected challenges, promoting reliability and an improved user experience in dynamic and distributed environments.

## Polly

Polly is a .NET resilience and transient-fault-handling library that allows developers to express policies such as Retry, Circuit Breaker, Timeout, Bulkhead Isolation, Rate-limiting and Fallback in a fluent and thread-safe manner.

<p align="start">
  <img src="./assets/polly-img.png" width="100" />
</p>

## Test

To run this project you need docker installed on your machine, see the docker documentation [here](https://www.docker.com/).

Having all the resources installed, run the command in a terminal from the root folder of the project and wait some seconds to build project image and download the resources:
`docker-compose up -d`

In terminal show this:

```console
 ✔ Network dotnet-idempotent_default    Created          0.8s
 ✔ Container dotnet-idempotent-redis-1  Started          1.4s
 ✔ Container idempotent_app             Started          2.4s
```

After this, access the link below:

- Swagger project [click here](http://localhost:5000/swagger)

### Stop Application

To stop, run: `docker-compose down`

## How implement

To implement, first install [the polly package](https://www.nuget.org/packages/Polly/8.0.0-alpha.8).

### Retry

```c#
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
```

### Wait and Retry

After three attempts, stop and issue an error.

```c#
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
```

Try again until you get a successful response.

```c#
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
```

### Fallback

```c#
var policy = Policy<string>
    .Handle<DomainException>(x => x.Code == "095")
    .FallbackAsync<string>((ct) => _test5Service.Execute2());

var result = await policy.ExecuteAsync(() => _test5Service.Execute1());

return Ok(result);
```
