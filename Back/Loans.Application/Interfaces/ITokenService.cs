using Loans.Application.DTOs;

namespace Loans.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(UserDto user);
}
