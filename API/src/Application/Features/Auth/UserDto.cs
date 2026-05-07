using PersonalFinanceAPI.Domain.Entities.Interfaces;

namespace PersonalFinanceAPI.Application.Features.Auth;

public record UserDto : IdDto<Guid>
{
    public string Email { get; init; } = string.Empty;
    public string Nickname { get; init; } = string.Empty;
    public string Currency { get; init; } = "BRL";
    public bool DarkTheme { get; init; } = false;
}
