using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Tests.Tags;

public class RoleTagTests
{
    [Theory]
    [InlineData("UserRole")]
    [InlineData("_Role1")]
    [InlineData("@Admin")]
    [InlineData("#TempRole")]
    [InlineData("Role$Name")]
    public void ValidRoleTag_ShouldConstructSuccessfully(string validRole)
    {
        // Arrange & Act
        RoleTag roleTag = new(validRole);

        // Assert implicit conversion to string and ToString() returns the original role name.
        string roleAsString = roleTag; // implicit conversion
        Assert.Equal(validRole, roleAsString);
        Assert.Equal(validRole, roleTag.ToString());
    }

    [Theory]
    [InlineData("Invalid Role")]           // Contains a space.
    [InlineData("123Role")]                // Starts with a digit.
    [InlineData("")]                       // Empty string.
    [InlineData("Role; DROP TABLE Users")] // Injection attempt.
    public void InvalidRoleTag_ShouldThrowSqlNamePatternException(string invalidRole) =>
        // Assert that constructing a RoleTag with an invalid role name throws the expected exception.
        Assert.Throws<InvalidSqlIdentifierException>(() => new RoleTag(invalidRole));

    [Fact]
    public void CompareTo_SameRole_ShouldReturnZero()
    {
        // Arrange
        RoleTag roleTag1 = new("UserRole");
        RoleTag roleTag2 = new("UserRole");

        // Act
        int result = roleTag1.CompareTo(roleTag2);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareTo_DifferentRoles_ShouldReturnNonZero()
    {
        // Arrange
        RoleTag roleTag1 = new("UserRole");
        RoleTag roleTag2 = new("_Admin");

        // Act
        int result = roleTag1.CompareTo(roleTag2);

        // Assert
        Assert.NotEqual(0, result);
    }

    [Fact]
    public void Equals_TwoEqualRoleTags_ShouldReturnTrue()
    {
        // Arrange
        RoleTag roleTag1 = new("UserRole");
        RoleTag roleTag2 = new("UserRole");

        // Act & Assert
        Assert.True(roleTag1.Equals(roleTag2));
        Assert.True(roleTag1.Equals((object)roleTag2));
        Assert.True(roleTag1 == roleTag2);
        Assert.False(roleTag1 != roleTag2);
    }

    [Fact]
    public void Equals_TwoDifferentRoleTags_ShouldReturnFalse()
    {
        // Arrange
        RoleTag roleTag1 = new("UserRole");
        RoleTag roleTag2 = new("_Admin");

        // Act & Assert
        Assert.False(roleTag1.Equals(roleTag2));
        Assert.False(roleTag1.Equals((object)roleTag2));
        Assert.False(roleTag1 == roleTag2);
        Assert.True(roleTag1 != roleTag2);
    }

    [Fact]
    public void GetHashCode_SameForEqualRoleTags()
    {
        // Arrange
        RoleTag roleTag1 = new("UserRole");
        RoleTag roleTag2 = new("UserRole");

        // Act
        int hash1 = roleTag1.GetHashCode();
        int hash2 = roleTag2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ToString_ShouldReturnRoleName()
    {
        // Arrange
        RoleTag roleTag = new("UserRole");

        // Act
        string result = roleTag.ToString();

        // Assert
        Assert.Equal("UserRole", result);
    }

    [Fact]
    public void ImplicitConversionToString_ShouldReturnRoleName()
    {
        // Arrange
        RoleTag roleTag = new("UserRole");

        // Act
        string result = roleTag; // implicit conversion to string

        // Assert
        Assert.Equal("UserRole", result);
    }
}
