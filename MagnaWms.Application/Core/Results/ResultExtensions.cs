using MagnaWms.Application.Core.Errors;

namespace MagnaWms.Application.Core.Results;

public static class ResultExtensions
{
    public static TResult Match<T, TResult>(
        this Result<T> result,
        Func<T, TResult> onSuccess,
        Func<Error, TResult> onFailure)
        => result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
}
