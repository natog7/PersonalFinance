using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Features.Shared;

public record IdDto<T>
{
	public required T Id { get; set; }
}
