using backend.Models;

namespace backend.Interfaces;

public interface IEcoPointCommand<TResult>
{
    Task<TResult> ExecuteAsync(IEcoPointTransactions receiver);
}