using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Domain.Services;

public interface ICurrentUserService
{
	Guid? UserId { get; }
	bool isAuthenticated { get; }
}
