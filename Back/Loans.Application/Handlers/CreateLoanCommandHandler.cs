using Loans.Application.Caching;
using Loans.Application.Commands;
using Loans.Application.DTOs;
using Loans.Application.Exceptions;
using Loans.Application.Interfaces;
using Loans.Domain.Entities;

namespace Loans.Application.Handlers;

public class CreateLoanCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
{
    public async Task<LoanDto> HandleAsync(CreateLoanCommand command)
    {
        var user = await unitOfWork.Users.GetByIdAsync(command.UserId)
            ?? throw new NotFoundException($"User '{command.UserId}' not found.");

        var loan = Loan.Create(command.UserId, command.Amount, command.TermInMonths, command.Purpose);

        await unitOfWork.Loans.AddAsync(loan);
        await unitOfWork.CommitAsync();

        cacheService.Remove(CacheKeys.UserLoans(command.UserId));
        cacheService.Remove(CacheKeys.AllLoans());

        return new LoanDto(
            loan.Id,
            loan.UserId,
            user.FullName,
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
