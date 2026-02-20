using PersonalFinanceAPI.Application.Features.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Features.Shared;

public class ListResult<T> where T : class
{
	public List<T> Items { get; set; } = new();
}
