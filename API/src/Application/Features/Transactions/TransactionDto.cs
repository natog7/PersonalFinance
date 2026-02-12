namespace PersonalFinanceAPI.Application.Features.Transactions;

	public record TransactionDto : IdDto<Guid>
{
		public string Title { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public string Currency { get; set; } = string.Empty;
		public DateOnly Date { get; set; }
		public int Type { get; set; }
		public Guid CategoryId { get; set; }
		public string CategoryName { get; set; } = string.Empty;
		public bool IsRecurrent { get; set; }
	}
