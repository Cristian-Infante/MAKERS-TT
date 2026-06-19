using Loans.Api.Contracts;
using Loans.Application.Commands;
using Loans.Application.DTOs;

namespace Loans.Api.Mapping;

public static class LoanMapper
{
    public static CreateLoanCommand ToCommand(this CreateLoanRequest request, Guid userId) =>
        new(userId, request.Amount, request.TermInMonths, request.Purpose);

    public static UpdateLoanStateCommand ToCommand(
        this UpdateLoanStateRequest request, Guid loanId, Guid requestingUserId) =>
        new(loanId, requestingUserId, request.Status, request.RejectionReason);

    public static LoanResponse ToResponse(this LoanDto dto) =>
        new(dto.Id, dto.UserId, dto.UserName, dto.Amount, dto.TermInMonths,
            dto.Purpose, dto.Status, dto.CreatedAt, dto.ApprovedAt,
            dto.RejectedAt, dto.RejectionReason);
}
