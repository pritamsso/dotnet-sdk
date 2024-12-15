# JWT Revoke .NET SDK

A .NET SDK for the JWT Revoke API that provides easy integration with token revocation services. Built with resilience and reliability in mind using Polly for retries, circuit breaking, and timeout handling.

## Installation

Install the package via NuGet:

dotnet add package JwtRevoke

Or add it directly to your project file:

<PackageReference Include="JwtRevoke" Version="1.0.0" />
<PackageReference Include="Microsoft.Extensions.Http.Resilience" Version="9.0.0" />

## Features

- Automatic retry with exponential backoff
- ⚡ Circuit breaker pattern for fault handling
- ⏱️ Configurable timeouts
- 🔒 Built-in rate limiting
- 🛡️ Type-safe error handling
- 🎯 Async/await support
- 📦 Dependency injection support

## Usage

### Basic Setup

using JwtRevoke;

// Initialize with default options
var client = new JwtRevokeClient("your_api_key_here", services);

// Or initialize with custom options
var options = new JwtRevokeOptions
{
    MaxRetries = 3,
    Timeout = TimeSpan.FromSeconds(10),
    RateLimitRetryDelay = TimeSpan.FromSeconds(1),
    BaseUrl = "https://api.jwtrevoke.com"
};

var clientWithOptions = new JwtRevokeClient("your_api_key_here", services, options);

### List Revoked Tokens

try
{
    var tokens = await client.ListRevokedTokensAsync();
    foreach (var token in tokens.Data)
    {
        Console.WriteLine($"Token ID: {token.Id}, Reason: {token.Reason}");
    }
}
catch (JwtRevokeException ex)
{
    Console.WriteLine($"API Error: {ex.Message} (Status: {ex.StatusCode})");
    Console.WriteLine($"Response Data: {ex.ResponseData}");
}

### Revoke a Token

try
{
    var revokedToken = await client.RevokeTokenAsync(
        jwtId: "token_123",
        reason: "Security breach",
        expiryDate: new DateTime(2024, 12, 31, 23, 59, 59)
    );
}
catch (JwtRevokeException ex)
{
    Console.WriteLine($"API Error: {ex.Message} (Status: {ex.StatusCode})");
    Console.WriteLine($"Response Data: {ex.ResponseData}");
}

### Delete a Revoked Token

try
{
    await client.DeleteRevokedTokenAsync("token_123");
}
catch (JwtRevokeException ex)
{
    Console.WriteLine($"API Error: {ex.Message} (Status: {ex.StatusCode})");
    Console.WriteLine($"Response Data: {ex.ResponseData}");
}

## Configuration Options

| Option | Description | Default |
|--------|-------------|---------|
| MaxRetries | Maximum number of retry attempts | 3 |
| Timeout | Request timeout duration | 10 seconds |
| RateLimitRetryDelay | Delay between rate limit retries | 1 second |
| BaseUrl | API base URL | https://api.jwtrevoke.com |

## Error Handling

The SDK uses the JwtRevokeException class for error handling, which includes:

- Message: Human-readable error message
- StatusCode: HTTP status code
- ResponseData: Raw response data from the API

## Dependencies

- Microsoft.Extensions.DependencyInjection.Abstractions (>= 9.0.0)
- Microsoft.Net.Http (>= 2.2.29)
- Polly (>= 8.5.0)
- Polly.Extensions.Http (>= 3.0.0)
- System.Text.Json (>= 9.0.0)

## Best Practices

1. API Key Security: Store your API key securely in configuration and never commit it to source control
2. Error Handling: Always wrap API calls in try-catch blocks to handle potential errors
3. Timeout Configuration: Adjust timeouts based on your application's needs
4. Dependency Injection: Use the SDK with your application's DI container

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support, please open an issue in the GitHub repository or contact our support team.