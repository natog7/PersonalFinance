using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Features.Transactions;

public record UpdateTransactionCommand(Guid Id, string Title, decimal Amount, string Currency, DateOnly Date, int Type, Guid CategoryId) : IRequest<TransactionDto>;
