namespace Loans.Application.Caching;

public static class CacheKeys
{
    public static string UserLoans(Guid userId) => $"loans:user:{userId}";
    public static string AllLoans() => "loans:all";
}
