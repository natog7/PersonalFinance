using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Domain.Services;

public interface ICurrentUserService
{
	Guid? UserId { get; }
	string Currency { get; }
	bool isAuthenticated { get; }
}
