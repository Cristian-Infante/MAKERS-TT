using Loans.Application.Interfaces;
using Loans.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;

namespace Loans.Infrastructure.Data;

public sealed class UnitOfWork : IUnitOfWork, IDisposable
{
    internal SqlConnection Connection { get; }
    internal SqlTransaction Transaction { get; private set; }

    private IUserRepository? _users;
    private ILoanRepository? _loans;

    public UnitOfWork(SqlConnectionFactory factory)
    {
        Connection = factory.CreateConnection();
        Connection.Open();
        Transaction = Connection.BeginTransaction();
    }

    public IUserRepository Users => _users ??= new UserRepository(this);
    public ILoanRepository Loans => _loans ??= new LoanRepository(this);

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await Transaction.CommitAsync(cancellationToken);
        await Transaction.DisposeAsync();
        Transaction = Connection.BeginTransaction();
        _users = null;
        _loans = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await Transaction.RollbackAsync(cancellationToken);
        await Transaction.DisposeAsync();
        Transaction = Connection.BeginTransaction();
        _users = null;
        _loans = null;
    }

    public void Dispose()
    {
        Transaction.Dispose();
        Connection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Transaction.DisposeAsync();
        await Connection.DisposeAsync();
    }
}
