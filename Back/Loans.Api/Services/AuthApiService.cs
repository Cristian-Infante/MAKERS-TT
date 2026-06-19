using Loans.Api.Contracts;
using Loans.Api.Mapping;
using Loans.Application.DTOs;
using Wolverine;

namespace Loans.Api.Services;

public sealed class AuthApiService(IMessageBus bus) : IAuthApiService
{
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var result = await bus.InvokeAsync<AuthResultDto>(request.ToCommand());
        return result.ToResponse();
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var result = await bus.InvokeAsync<AuthResultDto>(request.ToCommand());
        return result.ToResponse();
    }
}
