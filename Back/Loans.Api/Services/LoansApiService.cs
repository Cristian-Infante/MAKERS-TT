using Loans.Api.Contracts;
using Loans.Api.Mapping;
using Loans.Application.Commands;
using Loans.Application.DTOs;
using Loans.Application.Queries;
using Wolverine;

namespace Loans.Api.Services;

public sealed class LoansApiService(IMessageBus bus) : ILoansApiService
{
    public async Task<LoanResponse> CreateAsync(CreateLoanRequest request, Guid userId)
    {
        var result = await bus.InvokeAsync<LoanDto>(request.ToCommand(userId));
        return result.ToResponse();
    }

    public async Task<IEnumerable<LoanResponse>> GetMyLoansAsync(Guid userId)
    {
        var result = await bus.InvokeAsync<IEnumerable<LoanDto>>(new GetMyLoansQuery(userId));
        return result.Select(dto => dto.ToResponse());
    }

    public async Task<IEnumerable<LoanResponse>> GetAdminLoansAsync()
    {
        var result = await bus.InvokeAsync<IEnumerable<LoanDto>>(new GetAdminLoansQuery());
        return result.Select(dto => dto.ToResponse());
    }

    public async Task<LoanResponse> UpdateStateAsync(
        Guid loanId, UpdateLoanStateRequest request, Guid requestingUserId)
    {
        var result = await bus.InvokeAsync<LoanDto>(request.ToCommand(loanId, requestingUserId));
        return result.ToResponse();
    }
}
