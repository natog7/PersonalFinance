using PersonalFinanceAPI.Domain.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Features.Shared;

public record IdDto<TId> : IEntityFields<TId> where TId : struct, IEquatable<TId>
{
	public required TId Id { get; set; }
}
