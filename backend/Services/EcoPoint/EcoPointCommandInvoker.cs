using backend.Data;
using backend.Interfaces;
using Npgsql;

namespace backend.Services.EcoPoint;

public class EcoPointCommandInvoker
{
    private readonly IEcoPointTransactions _receiver;
    private readonly ApplicationDbContext _database;
    private const int MaxRetries = 3;

    public EcoPointCommandInvoker(IEcoPointTransactions receiver, ApplicationDbContext database)
    {
        _receiver = receiver;
        _database = database;
    }

    public async Task<TResult> InvokeAsync<TResult>(IEcoPointCommand<TResult> command)
    {
        var attempt = 0;
        while (true)
        {
            await using var transaction = await _database.Database.BeginTransactionAsync();
            try
            {
                var result = await command.ExecuteAsync(_receiver);
                await transaction.CommitAsync();
                return result;
            }
            catch (PostgresException ex) when (IsTransient(ex) && attempt < MaxRetries)
            {
                await transaction.RollbackAsync();
                attempt++;
                await Task.Delay(TimeSpan.FromMilliseconds(200 * attempt));
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    private static bool IsTransient(PostgresException ex) =>
        ex.SqlState is "40001" or "40P01"; //SQL errors for serialization failure or deadlock
}