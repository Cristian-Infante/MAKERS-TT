using Loans.Application.Caching;
using Loans.Application.DTOs;
using Loans.Application.Interfaces;
using Loans.Application.Queries;

namespace Loans.Application.Handlers;

public class GetMyLoansQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
{
    public async Task<IEnumerable<LoanDto>> HandleAsync(GetMyLoansQuery query)
    {
        var cached = cacheService.Get<List<LoanDto>>(CacheKeys.UserLoans(query.UserId));
        if (cached is not null)
            return cached;

        var loans = (await unitOfWork.Loans.GetByUserIdAsync(query.UserId)).ToList();
        cacheService.Set(CacheKeys.UserLoans(query.UserId), loans);

        return loans;
    }
}
