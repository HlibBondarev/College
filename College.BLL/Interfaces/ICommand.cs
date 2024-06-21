using MediatR;

namespace College.BLL.Interfaces;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
