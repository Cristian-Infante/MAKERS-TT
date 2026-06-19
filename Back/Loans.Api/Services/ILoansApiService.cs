using Loans.Api.Contracts;

namespace Loans.Api.Services;

public interface ILoansApiService
{
    Task<LoanResponse> CreateAsync(CreateLoanRequest request, Guid userId);
    Task<IEnumerable<LoanResponse>> GetMyLoansAsync(Guid userId);
    Task<IEnumerable<LoanResponse>> GetAdminLoansAsync();
    Task<LoanResponse> UpdateStateAsync(Guid loanId, UpdateLoanStateRequest request, Guid requestingUserId);
}
