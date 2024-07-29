using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AwsLambdaDotnet;

public class Function
{
    private readonly IServiceProvider _serviceProvider;

    public Function()
    {
        _serviceProvider = Startup.BuildServices();
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public string FunctionHandler(string input, ILambdaContext context)
    {
        var requestId = string.Equals(Startup.EnvironmentName, "Local")
            ? Guid.NewGuid().ToString()
            : context.AwsRequestId;

        var logger = _serviceProvider.GetRequiredService<ILogger<Function>>();

        logger.LogInformation($"Handling request: {requestId}");

        return input.ToUpper();
    }
}
