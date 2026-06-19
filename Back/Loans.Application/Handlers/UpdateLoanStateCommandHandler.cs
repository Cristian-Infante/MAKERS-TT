using Loans.Application.Caching;
using Loans.Application.Commands;
using Loans.Application.DTOs;
using Loans.Application.Exceptions;
using Loans.Application.Interfaces;
using Loans.Domain.Enums;
using Loans.Domain.Exceptions;

namespace Loans.Application.Handlers;

public class UpdateLoanStateCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
{
    public async Task<LoanDto> HandleAsync(UpdateLoanStateCommand command)
    {
        var loan = await unitOfWork.Loans.GetByIdAsync(command.LoanId)
            ?? throw new NotFoundException($"Loan '{command.LoanId}' not found.");

        if (!Enum.TryParse<LoanStatus>(command.Status, ignoreCase: false, out var status))
            throw new DomainException($"Invalid loan status '{command.Status}'.");

        switch (status)
        {
            case LoanStatus.Approved:
                loan.Approve();
                break;
            case LoanStatus.Rejected:
                loan.Reject(command.RejectionReason ?? string.Empty);
                break;
            default:
                throw new DomainException($"Cannot transition loan to status '{command.Status}'.");
        }

        await unitOfWork.Loans.UpdateAsync(loan);
        await unitOfWork.CommitAsync();

        cacheService.Remove(CacheKeys.UserLoans(loan.UserId));
        cacheService.Remove(CacheKeys.AllLoans());

        var user = await unitOfWork.Users.GetByIdAsync(loan.UserId);

        return new LoanDto(
            loan.Id,
            loan.UserId,
            user?.FullName ?? string.Empty,
            loan.Amount,
            loan.TermInMonths,
            loan.Purpose,
            loan.Status.ToString(),
            loan.CreatedAt,
            loan.ApprovedAt,
            loan.RejectedAt,
            loan.RejectionReason);
    }
}
