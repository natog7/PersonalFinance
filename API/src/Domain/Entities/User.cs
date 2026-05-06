using PersonalFinanceAPI.Domain.Enums;

namespace PersonalFinanceAPI.Domain.Entities;

public class User : Entity<Guid>
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Nickname { get; private set; } = string.Empty;
    public string Currency { get; private set; } = "BRL";
    public bool DarkTheme { get; private set; } = false;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private User() { }

    public static User Create(string email, string passwordHash, string nickname)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password is required.", nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(nickname))
            throw new ArgumentException("Full name is required.", nameof(nickname));

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLower().Trim(),
            PasswordHash = passwordHash,
            Nickname = nickname.Trim(),
            Currency = "BRL",
            DarkTheme = false,
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void Update(string? email, string? nickname, string? currency, bool? darkTheme)
    {
        if (!string.IsNullOrWhiteSpace(email))
            Email = email.ToLower().Trim();

        if (!string.IsNullOrWhiteSpace(nickname))
            Nickname = nickname.Trim();

        if (!string.IsNullOrWhiteSpace(currency))
            Currency = currency.ToUpper().Trim();

        if (darkTheme.HasValue)
            DarkTheme = darkTheme.Value;
    }
}
