namespace PersonalFinanceAPI.Domain.Entities;

/// <summary>
/// Represents a transaction category.
/// </summary>
public class Category : Entity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
	public string Color { get; private set; } = "#000000";
	public Guid? ParentCategoryId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation properties
    public Category? ParentCategory { get; private set; }
    public ICollection<Category> Subcategories { get; private set; } = new List<Category>();
    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    private Category() { }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    public static Category Create(string name, string? description = null, string color = "#000000", Guid? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty.", nameof(name));

        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim(),
			Color = ValidateColor(color),
			ParentCategoryId = parentCategoryId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

	/// <summary>
	/// Activates/Deactivates the category.
	/// </summary>
	public void Activate(bool active) => IsActive = active;

	protected static string ValidateColor(string color)
	{
		if (string.IsNullOrWhiteSpace(color))
			return "#000000";

		color = color.Trim();
		if (!System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9a-fA-F]{6}$"))
			throw new ArgumentException("Color must be a valid hex color code (e.g., #RRGGBB).", nameof(color));

		return color;
	}
}
