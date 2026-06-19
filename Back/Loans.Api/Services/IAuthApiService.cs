using Loans.Api.Contracts;

namespace Loans.Api.Services;

public interface IAuthApiService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
}
