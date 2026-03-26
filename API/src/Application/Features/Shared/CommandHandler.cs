using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Shared;

public class CommandHandler<TRequest, TResponse, TRepository> : IRequestHandler<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	protected readonly TRepository _repository;
	protected readonly ICurrentUserService _userService;

	public CommandHandler(TRepository repository, ICurrentUserService userService)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		_userService = userService ?? throw new ArgumentNullException(nameof(userService));
	}

	public virtual async Task<TResponse> Handle(TRequest request, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public virtual void CheckAuthenticated()
	{
		if (!_userService.isAuthenticated)
		{
			throw new UnauthorizedAccessException("User must be authenticated by logging in.");
		}
	}
}