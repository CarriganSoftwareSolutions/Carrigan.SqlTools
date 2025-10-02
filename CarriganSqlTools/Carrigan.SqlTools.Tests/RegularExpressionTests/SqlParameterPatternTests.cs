using Carrigan.SqlTools.RegularExpressions;

namespace Carrigan.SqlTools.Tests.RegularExpressionTests;

//ignore spelling: Имя 😀abc Nl

public class SqlParameterPatternTests
{

    [Theory]
    [InlineData("UserRole")]
    [InlineData("_Role1")]
    [InlineData("#TempRole")]
    [InlineData("$TempRole")]
    [InlineData("@TempRole")]
    [InlineData("@@TempRole")]
    [InlineData("@@@TempRole")]
    [InlineData("Role$Name")]
    [InlineData("@name")]
    [InlineData("123Invalid")]           //Note: @ gets added to the start of parameters, so this is actually fine
    [InlineData("١abc")]                 // starts with digit (Arabic-Indic)
    //Tests suggested by AI:
    [InlineData("Δ")]                    // Greek letter start
    [InlineData("Имя")]                  // Cyrillic
    [InlineData("名")]                   // CJK
    [InlineData("_Δ")]                   // underscore start + Greek
    [InlineData("A١٢٣")]                 // Arabic-Indic digits allowed after first char
    [InlineData("A_B_C")]                // underscores are all correct
    [InlineData("A$B#C@D")]
    [InlineData("ab@c")]
    //___________00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000011111111111111111111111111111
    //___________00000000011111111112222222222333333333344444444445555555555666666666677777777778888888888999999999900000000001111111111222222222
    [InlineData("_2345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678")] //size limit
    public void ValidIdentifiers_ShouldPass(string identifier)
    {
        // Assert that the identifier passes the pattern.
        Assert.True(SqlParameterPattern.Passes(identifier));
        // And that it does not fail.
        Assert.False(SqlParameterPattern.Fails(identifier));
    }

    [Theory]
    [InlineData("Invalid Role")]            // Contains a space.
    [InlineData("Role; DROP TABLE Users")]  // Contains SQL injection payload.
    [InlineData("User-Role")]               // Contains a hyphen.
    [InlineData("")]                        // Empty string.
    [InlineData(" ")]                       // A string with just whitespace.
    [InlineData(null)]                      // null
    [InlineData("@@@@@@@")]                 // null

    //tests suggested by AI:
    [InlineData("😀abc")]                   // emoji start not a letter/underscore
    [InlineData("\u0301a")]                 // starts with combining mark (invalid)
    [InlineData("A Name")]                  // space (already covered similar, but explicit Unicode set unchanged)
    [InlineData("A.Name")]                  // dot not allowed in unquoted identifier
    [InlineData("A-Name")]                  // hyphen not allowed (already similar covered)
    [InlineData("A½")]                      // Unicode "other" number
    [InlineData("A²")]                      // Unicode "other" number
    [InlineData("A①")]                     // Unicode "other" number
    [InlineData("A\u0301")]                 // A + ◌́ decomposed (has \p{M} combining marks) → invalid unquoted
    [InlineData("o\u0308")]                 // o + ◌̈ decomposed (has \p{M} combining marks) → invalid unquoted
    [InlineData("e\u0301\u0323")]           // e + acute + dot-below (even NFC often leaves one \p{M})
    [InlineData("A𝟡")]                      // Mathematical double-struck digit 9 (Unicode Nd)
    //___________000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000111111111111111111111111111111
    //___________000000000111111111122222222223333333333444444444455555555556666666666777777777788888888889999999999000000000011111111112222222222
    [InlineData("_23456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789")] //size limit
    public void InvalidIdentifiers_ShouldFail(string? identifier)
    {
        // Assert that the identifier does not pass the pattern.
        Assert.False(SqlParameterPattern.Passes(identifier));
        // And that it does fail.
        Assert.True(SqlParameterPattern.Fails(identifier));
    }
}
