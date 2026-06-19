using Loans.Application.DTOs;
using Loans.Domain.Entities;

namespace Loans.Application.Interfaces;

public interface ILoanRepository
{
    Task<IEnumerable<LoanDto>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<LoanDto>> GetAllWithUserAsync();
    Task<Loan?> GetByIdAsync(Guid id);
    Task AddAsync(Loan loan);
    Task UpdateAsync(Loan loan);
}
