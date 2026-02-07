using System.Security.Cryptography;

namespace FormulaTests;

using Formula;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class FormulaValidity
{
    [TestMethod]
    public void FormulaConstructor_TestNoTokens_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula(""));
        // note: it is arguable that you should replace "" with string.Empty for readability and clarity of intent (e.g., not a cut-and-paste error or a "I forgot to put something there" error).
    }


    // --- Tests for Valid Token Rule ---

    // Test for validating number/symbol token 
    [TestMethod]
    public void FormulaConstructor_TestToken_Valid()
    {
        _ = new Formula("1");
    }

    [TestMethod]
    public void FormulaConstructor_TestDoubleOp_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("1++1"));
    }

    // (, ), +, -, *, / are only valid symbols
    [TestMethod]
    public void FormulaConstructor_TestToken_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("1 & 2"));
    }

    // Expects error for having a variable that could also be a cell for future spreadsheet
    [TestMethod]
    public void FormulaConstructor_TestTokenVariable_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("a1a"));
    }

    [TestMethod]
    public void FormulaConstructor_TestTokenLetter_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("a"));
    }

    // Scientific Notation validity test
    [TestMethod]
    public void FormulaConstructor_TestTokenScientificNotation_Valid()
    {
        _ = new Formula("2e7");
    }


    // --- Tests for Closing Parenthesis Rule

    // Method that checks for the same amount of closing parentheses as opening parentheses 
    [TestMethod]
    public void FormulaConstructor_TestClosingParentheses_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("(2+1))"));
    }

    // Valid formula with same number closing parenthesis as opening
    [TestMethod]
    public void FormulaConstructor_TestClosingParentheses_Valid()
    {
        _ = new Formula("(2+1)");
    }

    // --- Tests for Balanced Parentheses Rule

    // Test that expects a formula exception because there is no closing parenthesis
    [TestMethod]
    public void FormulaConstructor_TestBalancedParenthesesClosingMissing_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("( 1+1"));
    }

    // Test that expects a formula exception because there are more opening parentheses than closing
    [TestMethod]
    public void FormulaConstructor_TestBalancedParenthesesOpenHeavy_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("(( 1+1"));
    }

    // Equal number of opening and closing parentheses
    [TestMethod]
    public void FormulaConstructor_TestBalancedParentheses_Valid()
    {
        _ = new Formula("(1 + ( 1+1 ) + 1)");
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
    public void FormulaConstructor_TestFirstTokenNumber_Valid()
    {
        _ = new Formula("1+1");
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
    public void FormulaConstructor_TestFirstTokenOpeningParenthesis_Valid()
    {
        _ = new Formula("(1 + 1)");
    }

    // Expected to throw an exception for invalid first token being an operator
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenOperator_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("*1 + 1"));
    }

    // --- Tests for  Last Token Rule ---

    // Test that shows a number to end a formula is valid even if just a number
    [TestMethod]
    public void FormulaConstructor_TestLastTokenNumber_Valid()
    {
        _ = new Formula("1");
    }

    // Test that expects a formula exceptions because an operator to end is invalid
    [TestMethod]
    public void FormulaConstructor_TestLastTokenOperator_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("1 + 1 +"));
    }

    //Test that expects formula exception because last operator is an opening parenthesis
    [TestMethod]
    public void FormulaConstructor_TestLastTokenOpenPara_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("1 + 1("));
    }

    // --- Tests for Parentheses/Operator Following Rule ---
    [TestMethod]
    public void FormulaConstructor_TestParenthesisFollowing_Valid()
    {
        _ = new Formula("(1+1) + 1");
    }

    // Test that expects formula exception to be thrown because an operator after an opening parenthesis
    // is invalid
    [TestMethod]
    public void FormulaConstructor_TestParenthesisFollowing_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("(+1)"));
    }


    // --- Tests for Extra Following Rule ---

    // Test that expects formula exception to be thrown because a number following a closing parenthesis
    // is not valid
    [TestMethod]
    public void FormulaConstructor_TestExtraFollowing_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("(1 + 1)1"));
    }

    // Expects formula exception for having nothing within the parentheses
    [TestMethod]
    public void FormulaConstructor_TestExtraFollowingEmptyParentheses_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("() + 1"));
    }

    // Expects error for having a lone operator after parentheses
    [TestMethod]
    public void FormulaConstructor_TestExtraFollowingLoneOperatior_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("(1 + 1)+"));
    }

    // Expects error if parenthesis are back to back with no operator between them
    [TestMethod]
    public void FormulaConstructor_TestExtraFollowingMultiplyingParentheses_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("(1 + 1)(1 + 1)"));
    }

    // Tests ToString method to represent formula in canonical form
    [TestMethod]
    public void ToString_Valid()
    {
        Formula testFormulaInput = new Formula("1a2 + 2 * 3e6");
        Assert.AreEqual("1A2+2*3000000", testFormulaInput.ToString());
    }

    // Tests GetVariables method to ensure variables are contained in a hash set
    [TestMethod]
    public void GetVariables_Valid()
    {
        HashSet<string> variablesSet = new HashSet<string>(new[] { "1a1, 2p3, 9www8" });
        Formula testVariables = new Formula("1a1 + 2p3 + 9www8");
        Assert.AreEqual(variablesSet.ToString(), testVariables.GetVariables().ToString());
    }

    // Tests GetVariables method to show that the hash set does not store duplicates
    [TestMethod]
    public void GetVariablesWithDuplicates_Valid()
    {
        HashSet<string> _ = new HashSet<string>(new[] { "1a1, 2p3, 9www8," });
        Formula duplicateVariables = new Formula("1a1 + 2p3 + 9www8 + 1a1");
        Assert.AreEqual(_.ToString(), duplicateVariables.GetVariables().ToString());
    }

    /// <summary>
    /// Expects equal operator to report true when two formulas are the same
    /// </summary>
    [TestMethod]
    public void FormulasAreEqual_EqualEqualOverride()
    {
        Formula f1 = new Formula("1a2 + 2 * 3e6");
        Formula f2 = new Formula("1a2 + 2 * 3e6");
        Assert.IsTrue(f1 == f2);
    }

    /// <summary>
    /// Expects equal operator to report false when two formulas are different
    /// </summary>
    [TestMethod]
    public void FormulasAreNotEqual_EqualEqualOverride()
    {
        Formula f1 = new Formula("1a2 + 2 * 3e6");
        Formula f2 = new Formula("1a2 + 2 * 3e5");
        Assert.IsFalse(f1 == f2);
    }

    /// <summary>
    /// Expects the not equals operator correctly reports false when two formulas are the same 
    /// </summary>
    [TestMethod]
    public void FormulasAreEqual_NotEqualOverride()
    {
        Formula f1 = new Formula("1a2 + 2 * 3e6");
        Formula f2 = new Formula("1a2 + 2 * 3e6");
        Assert.IsFalse(f1 != f2);
    }

    /// <summary>
    /// Expects the not equals operator correctly reports true when two formulas are different 
    /// </summary>
    [TestMethod]
    public void FormulasAreNotEqual_EqualNotOverride()
    {
        Formula f1 = new Formula("1a2 + 2 * 3e6");
        Formula f2 = new Formula("1a2 + 2 * 3e5");
        Assert.IsTrue(f1 != f2);
    }

    /// <summary>
    /// Expects that f1 HashCode and f2 HashCode are equal
    /// </summary>
    [TestMethod]
    public void GetHashCode_Equal()
    {
        Formula f1 = new Formula("1a2 + 2 * 3e6");
        Formula f2 = new Formula("1a2 + 2 * 3e6");
        Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
    }

    /// <summary>
    /// Expects that f1 HashCode and f2 HashCode are not equal
    /// </summary>
    [TestMethod]
    public void GetHashCode_NotEqual()
    {
        Formula f1 = new Formula("1a2 + 2 * 3e6");
        Formula f2 = new Formula("1a2 + 2 * 3e5");
        Assert.AreNotEqual(f1.GetHashCode(), f2.GetHashCode());
    }

    // Expects equals to report true because Formulas f1 and f2 are the same
    [TestMethod]
    public void Equals_SameFormula()
    {
        Formula f1 = new Formula("1a2 + 2 * 3e6");
        Formula f2 = new Formula("1a2 + 2 * 3e6");
        Assert.IsTrue(f1.Equals(f2));
    }

    // Expects equals to report false because Formulas f1 and f2 are not the same
    [TestMethod]
    public void Equals_DifferentFormula()
    {
        Formula f1 = new Formula("1a2 + 2 * 3e6");
        Formula f2 = new Formula("1a2 + 2 * 3e5");
        Assert.IsFalse(f1.Equals(f2));
    }

    // Expects false because Formula f1 and double f2 are the same type
    [TestMethod]
    public void Equals_DifferentTypes()
    {
        Formula f1 = new Formula("12.0");
        double f2 = 12.0;
        Assert.IsFalse(f1.Equals(f2));
    }

    // Expects Formula f1 and 12.0 to be equal after evaluating f1
    [TestMethod]
    public void Equals_AfterEvaluate()
    {
        Formula f1 = new Formula("A1");
        double f2 = 12.0;

        double TestLookup(string token)
        {
            if (token == "A1") 
                return 12.0;
            
            throw new ArgumentException("Invalid token");
        }
        Assert.IsTrue(f1.Evaluate(TestLookup).Equals(f2));
    }
}

/// <summary>
/// Test class that only contains tests for the different arithmetic in the Evaluate method.
/// </summary>
[TestClass]
public class EvaluateFormula
{
    /// <summary>
    /// Simple test for addition in the evaluate method. Expects that created cells A1 and B2 are
    /// correctly added.
    /// </summary>
    /// <exception cref="FormatException">Exception if lookup could not resolve a variable</exception>
    [TestMethod]
    public void TestLookup_VarPlusVar()
    {
        Formula f1 = new Formula("A1 + B2");
        
        double TestLookup(string token)
        {
            if (token == "A1") return 12.0;
            if (token == "B2") return 8.0;
            if (token == "C3") return 2.0;

            throw new ArgumentException("Invalid token");
        }

        object result = f1.Evaluate(TestLookup);
        Assert.AreEqual(20, (double)result);
    }

    /// <summary>
    /// Expects that created cell A1 and an arbitrary number will still add together correctly.
    /// </summary>
    /// <exception cref="FormatException">Exception if lookup could not resolve a variable</exception>
    [TestMethod]
    public void TestLookup_VarPlusNum()
    {
        Formula f1 = new Formula("A1 + 8");

        double TestLookup(string token)
        {
            if (token == "A1") return 12.0;
            if (token == "B2") return 8.0;
            if (token == "C3") return 2.0;

            throw new ArgumentException("Invalid token");
        }

        object result = f1.Evaluate(TestLookup);
        Assert.AreEqual(20, (double)result);
    }

    /// <summary>
    /// Expects that created cells A1 and B2 are multiplied together correctly.
    /// </summary>
    /// <exception cref="FormatException">Exception if lookup could not resolve a variable</exception>
    [TestMethod]
    public void TestLookup_Multiply()
    {
        Formula f1 = new Formula("A1 * B2");

        double TestLookup(string token)
        {
            if (token == "A1") return 12.0;
            if (token == "B2") return 8.0;
            if (token == "C3") return 2.0;

            throw new ArgumentException("Invalid token");
        }

        object result = f1.Evaluate(TestLookup);
        Assert.AreEqual(96, (double)result);
    }

    /// <summary>
    /// Expects that created cells A1, B2 and C3 are handled by Evaluate for the correct order of operations.
    /// </summary>
    /// <exception cref="FormatException">Exception if lookup could not resolve a variable</exception>
    [TestMethod]
    public void TestLookup_MultiplyAndAdd()
    {
        Formula f1 = new Formula("A1 * B2 + C3");

        double TestLookup(string token)
        {
            if (token == "A1") return 12.0;
            if (token == "B2") return 8.0;
            if (token == "C3") return 2.0;

            throw new ArgumentException("Invalid token");
        }

        object result = f1.Evaluate(TestLookup);
        Assert.AreEqual(98, (double)result);
    }

    /// <summary>
    /// Expects that created cells A1, B2 and C3 are handled by Evaluate for the correct order of operations.
    /// </summary>
    /// <exception cref="FormatException">Exception if lookup could not resolve a variable</exception>
    [TestMethod]
    public void TestLookup_AddThenMultiply()
    {
        Formula f1 = new Formula("A1 + B2 * C3");

        double TestLookup(string token)
        {
            if (token == "A1") return 12.0;
            if (token == "B2") return 8.0;
            if (token == "C3") return 2.0;

            throw new ArgumentException("Invalid token");
        }

        object result = f1.Evaluate(TestLookup);
        Assert.AreEqual(28, (double)result);
    }

    /// <summary>
    /// Expects that created cells A1 is divided by B2 correctly.
    /// </summary>
    /// <exception cref="FormatException">Exception if lookup could not resolve a variable</exception>
    [TestMethod]
    public void TestLookup_Divide()
    {
        Formula f1 = new Formula("A1 / B2");
        double answer = 12.0 / 8.0;

        double TestLookup(string token)
        {
            if (token == "A1") return 12.0;
            if (token == "B2") return 8.0;
            if (token == "C3") return 2.0;

            throw new ArgumentException("Invalid token");
        }

        object result = f1.Evaluate(TestLookup);
        Assert.AreEqual(answer, (double)result);
    }

    /// <summary>
    /// Expects that the correct order of operations is executed by created cells A1, B2 and C3
    /// </summary>
    /// <exception cref="FormatException">Exception if lookup could not resolve a variable</exception>
    [TestMethod]
    public void TestLookup_ParaThenDivide()
    {
        Formula f1 = new Formula("(A1 + B2) / C3");

        double TestLookup(string token)
        {
            if (token == "A1") return 12.0;
            if (token == "B2") return 8.0;
            if (token == "C3") return 2.0;

            throw new ArgumentException("Invalid token");
        }

        object result = f1.Evaluate(TestLookup);
        Assert.AreEqual(10, (double)result);
    }

    /// <summary>
    /// Expects that the correct order of operations is executed by created cells A1, B2 and C3 and arbitrary numbers
    /// </summary>
    /// <exception cref="FormatException">Exception if lookup could not resolve a variable</exception>
    [TestMethod]
    public void TestLookup_LongArithmetic()
    {
        Formula f1 = new Formula("(A1 + B2) * C3 - 10 + (A1 / C3) - 1");
        object result = f1.Evaluate(TestLookup);

        double TestLookup(string token)
        {
            if (token == "A1") return 12.0;
            if (token == "B2") return 8.0;
            if (token == "C3") return 2.0;

            throw new ArgumentException("Invalid token");
        }

        double answer = (12.0 + 8.0) * 2.0 - 10.0 + (12.0 / 2.0) - 1;
        Assert.AreEqual(35, (double)result);
    }

    /// <summary>
    /// Expects Evaluate method to return a FormulaError because cell A2 is not a cell within the Lookup
    /// </summary>
    /// <exception cref="FormatException">Exception if lookup could not resolve a variable</exception>
    [TestMethod]
    public void TestLookup_InvalidCell()
    {
        Formula f1 = new Formula("A1 + A2");

        double TestLookup(string token)
        {
            if (token == "A1") return 12.0;
            if (token == "B2") return 8.0;
            if (token == "C3") return 2.0;
            
            throw new ArgumentException($"Invalid token {token}");
        }
        
        Assert.IsInstanceOfType(f1.Evaluate(TestLookup), typeof(FormulaError));
    }

    /// <summary>
    /// Expects Evaluate method to return a FormulaError because A1 is being divided y 0
    /// </summary>
    /// <exception cref="FormatException">Exception if lookup could not resolve a variable</exception>
    [TestMethod]
    public void Evaluate_DivideByZero()
    {
        Formula f1 = new Formula("A1 / 0");

        double TestLookup(string token)
        {
            if (token == "A1") return 12.0;
            if (token == "B2") return 8.0;
            if (token == "C3") return 2.0;

            throw new ArgumentException("Invalid token");
        }

        Assert.IsInstanceOfType(f1.Evaluate(TestLookup), typeof(FormulaError));
    }

    /// <summary>
    /// Expects multiplying by 0 to return 0
    /// </summary>
    /// <exception cref="FormatException">Exception if lookup could not resolve a variable</exception>
    [TestMethod]
    public void Evaluate_MultiplyBy0()
    {
        Formula f1 = new Formula("A1 * 0");

        double TestLookup(string token)
        {
            if (token == "A1") return 12.0;
            if (token == "B2") return 8.0;
            if (token == "C3") return 2.0;

            throw new ArgumentException("Invalid token");
        }

        object result = f1.Evaluate(TestLookup);
        Assert.AreEqual(0, (double)result);
    }

    [TestMethod]
    public void Evaluate_LoneVariable()
    {
        Formula f1 = new Formula("A1");

        double TestLookup(string token)
        {
            if (token == "A1") return 12.0;
            
            throw new ArgumentException($"Invalid token {token}");
        }
        Assert.AreEqual(12.0, f1.Evaluate(TestLookup));
    }
    
}