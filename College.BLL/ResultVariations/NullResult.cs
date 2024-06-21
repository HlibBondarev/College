using FluentResults;

namespace College.BLL.ResultVariations;

public class NullResult<T> : Result<T>
{
    public NullResult()
        : base()
    {
    }
}
