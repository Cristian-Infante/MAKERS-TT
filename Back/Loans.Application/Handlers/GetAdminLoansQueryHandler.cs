using Loans.Application.Caching;
using Loans.Application.DTOs;
using Loans.Application.Interfaces;
using Loans.Application.Queries;

namespace Loans.Application.Handlers;

public class GetAdminLoansQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
{
    public async Task<IEnumerable<LoanDto>> HandleAsync(GetAdminLoansQuery query)
    {
        var cached = cacheService.Get<List<LoanDto>>(CacheKeys.AllLoans());
        if (cached is not null)
            return cached;

        var loans = (await unitOfWork.Loans.GetAllWithUserAsync()).ToList();
        cacheService.Set(CacheKeys.AllLoans(), loans);

        return loans;
    }
}
