namespace UnitTests.Domain;

public class CategoryTests
{
	[Fact]
	public void Create_WithValidData_ReturnsCategoryInstance()
	{
		// Act
		var category = Category.Create("Groceries", "Food expenses");

		// Assert
		Assert.NotEqual(Guid.Empty, category.Id);
		Assert.Equal("Groceries", category.Name);
		Assert.Equal("Food expenses", category.Description);
		Assert.True(category.IsActive);
		Assert.Equal(DateTime.UtcNow.Date, category.CreatedAt.Date);
	}

	[Fact]
	public void Create_WithEmptyName_ThrowsArgumentException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => Category.Create(""));
		Assert.Contains("Category name cannot be empty", ex.Message);
	}

	[Fact]
	public void Create_WithWhitespaceName_ThrowsArgumentException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => Category.Create("   "));
		Assert.Contains("Category name cannot be empty", ex.Message);
	}

	[Fact]
	public void Create_WithValidColor_ReturnsCategory()
	{
		// Act
		var category = Category.Create("Groceries", color: "#FF5733");

		// Assert
		Assert.Equal("#FF5733", category.Color);
	}

	[Fact]
	public void Create_WithInvalidColor_ThrowsArgumentException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => Category.Create("Test", color: "INVALID"));
		Assert.Contains("Color must be a valid hex color code", ex.Message);
	}

	[Fact]
	public void Create_WithDefaultColor_ReturnsBlackColor()
	{
		// Act
		var category = Category.Create("Groceries");

		// Assert
		Assert.Equal("#000000", category.Color);
	}

	[Fact]
	public void Create_TrimsCategoryName()
	{
		// Act
		var category = Category.Create("  Groceries  ");

		// Assert
		Assert.Equal("Groceries", category.Name);
	}

	[Fact]
	public void Create_WithParentCategoryId_ReturnsCategory()
	{
		// Arrange
		var parentId = Guid.NewGuid();

		// Act
		var category = Category.Create("Vegetables", parentCategoryId: parentId);

		// Assert
		Assert.Equal(parentId, category.ParentCategoryId);
	}

	[Fact]
	public void Activate_WithTrue_SetsIsActiveTrue()
	{
		// Arrange
		var category = Category.Create("Groceries");
		category.Activate(false);

		// Act
		category.Activate(true);

		// Assert
		Assert.True(category.IsActive);
	}

	[Fact]
	public void Activate_WithFalse_SetsIsActiveFalse()
	{
		// Arrange
		var category = Category.Create("Groceries");

		// Act
		category.Activate(false);

		// Assert
		Assert.False(category.IsActive);
	}
}
