using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Categories;

public class CategoryCommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
	protected readonly ICategoryRepository _repository;
	protected readonly ICurrentUserService _userService;

	public CategoryCommandHandler(ICategoryRepository repository, ICurrentUserService userService)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		_userService = userService ?? throw new ArgumentNullException(nameof(userService));
	}

	public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}