using MediatR;

namespace College.BLL.Interfaces;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
