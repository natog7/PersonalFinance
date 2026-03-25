using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PersonalFinanceAPI.Domain.Entities;

public abstract class UserEntity<TId> : Entity<TId> where TId : struct, IEquatable<TId>
{
	public TId UserId { get; protected set; }
}
