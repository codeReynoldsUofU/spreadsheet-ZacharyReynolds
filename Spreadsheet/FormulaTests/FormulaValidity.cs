namespace FormulaTests;

using Formula;


using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class FormulaValidity
{
     [TestMethod]
    public void FormulaConstructor_TestNoTokens_Invalid( )
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula(""));
        // note: it is arguable that you should replace "" with string.Empty for readability and clarity of intent (e.g., not a cut-and-paste error or a "I forgot to put something there" error).
    }

    
    // --- Tests for Valid Token Rule ---
    
    // Test for validating number/symbol token 
    [TestMethod]
    public void FormulaConstructor_TestToken_Valid()
    {
        _ = new Formula( "1" );
    }

    [TestMethod]
    public void FormulaConstructor_TestDoubleOp_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula( "1++1" ));
    }
    
    // (, ), +, -, *, / are only valid symbols
    [TestMethod]
    public void FormulaConstructor_TestToken_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula( "1 & 2" ));
    }
    
    // Expects error for having a variable that could also be a cell for future spreadsheet
    [TestMethod]
    public void FormulaConstructor_TestTokenVariable_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula( "a1a" ));
    }

    [TestMethod]
    public void FormulaConstructor_TestTokenLetter_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula( "a" ));
    }
    
    // Scientific Notation validity test
    [TestMethod]
    public void FormulaConstructor_TestTokenScientificNotation_Valid()
    {
       _ = new Formula( "2e7" );
    }
    
    
    // --- Tests for Closing Parenthesis Rule
    
    // Method that checks for the same amount of closing parentheses as opening parentheses 
    [TestMethod]
    public void FormulaConstructor_TestClosingParentheses_Invalid( )
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("(2+1))"));
    }

    // Valid formula with same number closing parenthesis as opening
    [TestMethod]
    public void FormulaConstructor_TestClosingParentheses_Valid()
    {
        _ = new Formula( "(2+1)" );
    }
    
    // --- Tests for Balanced Parentheses Rule
    
    // Test that expects a formula exception because there is no closing parenthesis
    [TestMethod]
    public void FormulaConstructor_TestBalancedParenthesesClosingMissing_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula( "( 1+1" ) );
    }

    // Test that expects a formula exception because there are more opening parentheses than closing
    [TestMethod]
    public void FormulaConstructor_TestBalancedParenthesesOpenHeavy_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula( "(( 1+1" ) );
    }
    // Equal number of opening and closing parentheses
    [TestMethod]
    public void FormulaConstructor_TestBalancedParentheses_Valid()
    {
        _ = new Formula( "(1 + ( 1+1 ) + 1)" );
    }

    // --- Tests for First Token Rule

    /// <summary>
    ///   <para>
    ///     Make sure a simple well-formed formula is accepted by the constructor (the constructor
    ///     should not throw an exception).
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
    ///     In other words, the formula "1+1" is a valid formula which should not cause any errors.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenNumber_Valid( )
    {
        _ = new Formula( "1+1" );
    }
    
    // <summary>
    //   <para>
    //     Make sure a simple well-formed formula is accepted by the constructor (the constructor
    //     should not throw an exception).
    //   </para>
    //   <remarks>
    //     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
    //     In other words, the formula "(1+1)" is a valid formula which should not cause any errors.
    //   </remarks>
    // </summary>
    
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenOpeningParenthesis_Valid( )
    {
        _ = new Formula( "(1 + 1)" );
    }
    
    // Expected to throw an exception for invalid first token being an operator
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenOperator_Invalid( )
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula( "*1 + 1" ));
    }

    // --- Tests for  Last Token Rule ---
    
    // Test that shows a number to end a formula is valid even if just a number
    [TestMethod]
    public void FormulaConstructor_TestLastTokenNumber_Valid( ) 
    {
        _ = new Formula( "1" );
    }
    
    // Test that expects a formula exceptions because an operator to end is invalid
    [TestMethod]
    public void FormulaConstructor_TestLastTokenOperator_Invalid( ) 
    {
        Assert.Throws<FormulaFormatException>(() =>  _ = new Formula( "1 + 1 +" ));
    }
    
    //Test that expects formula exception because last operator is an opening parenthesis
    [TestMethod]
    public void FormulaConstructor_TestLastTokenOpenPara_Invalid( ) 
    {
        Assert.Throws<FormulaFormatException>(() =>  _ = new Formula( "1 + 1(" ));
    }
    // --- Tests for Parentheses/Operator Following Rule ---
    [TestMethod]
    public void FormulaConstructor_TestParenthesisFollowing_Valid( )
    {
        _ = new Formula("(1+1) + 1");
    }

    // Test that expects formula exception to be thrown because an operator after an opening parenthesis
    // is invalid
    [TestMethod]
    public void FormulaConstructor_TestParenthesisFollowing_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula( "(+1)" ) );
    }
    
    
    // --- Tests for Extra Following Rule ---
    
    // Test that expects formula exeption to be thrown because a number following a closing parenthesis
    // is not valid
    [TestMethod]
    public void FormulaConstructor_TestExtraFollowing_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula( "(1 + 1)1" ) );
    }

    [TestMethod]
    public void ToString_Valid()
    {
       Formula _ = new Formula( "1a2 + 2 * 3e6" );
       Assert.AreEqual( "1A2+2*3000000", _.ToString( ) );
    }
}