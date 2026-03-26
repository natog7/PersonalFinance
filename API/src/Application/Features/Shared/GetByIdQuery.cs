namespace PersonalFinanceAPI.Application.Features.Shared;

public record GetByIdQuery<TResponse>(Guid Id) : IRequest<TResponse>;
