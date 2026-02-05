// <summary>
//   <para>
//     This code is provided to start your assignment.  It was written
//     by Profs Joe, Danny, Jim, and Travis.  You should keep this attribution
//     at the top of your code where you have your header comment, along
//     with any other required information.
//   </para>
//   <para>
//     You should remove/add/adjust comments in your file as appropriate
//     to represent your work and any changes you make.
//   </para>
// </summary>

using System.Reflection;
using System.Runtime.CompilerServices;

namespace Formula;

using System.Text.RegularExpressions;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one or more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
///   <para>
///     Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
///     a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
///     and "x 23" consists of a variable "x" and a number "23".  Otherwise, spaces are to be removed.
///   </para>
///   <para>
///     For Assignment Two, you are to implement the following functionality:
///   </para>
///   <list type="bullet">
///     <item>
///        Formula Constructor which checks the syntax of a formula.
///     </item>
///     <item>
///        Get Variables
///     </item>
///     <item>
///        ToString
///     </item>
///   </list>
/// </summary>
public class Formula
{
    /// <summary>
    ///   All variables are letters followed by numbers.  This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";

    private const string FirstTokenRegExPattern = @"\(|\d+|[a-zA-Z]+\d+|\d+[eE]?\d+";

    private const string LastTokenRegExPattern = @"\)|[0-9]+|[a-zA-Z]+\d+|\d+[eE]?\d+";

    private static List<string> _formulaTokens = [];

    private string _formulaString = "";

    private Stack<string> valueStack;

    private Stack<string> operatorStack;

    /// <summary>
    ///   Initializes a new instance of the <see cref="_formulaString"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment.  If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message.  See the assignment
    ///     specifications for the syntax rules you are to implement.
    ///   </para>
    ///   <para>
    ///     Non-Exhaustive Example Errors:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///        Invalid variable name, e.g., x, x1x  (Note: x1 is valid, but would be normalized to X1)
    ///     </item>
    ///     <item>
    ///        Empty formula, e.g., string.Empty
    ///     </item>
    ///     <item>
    ///        Mismatched Parentheses, e.g., "(("
    ///     </item>
    ///     <item>
    ///        Invalid Following Rule, e.g., "2x+5"
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula(string formula)
    {
        // Rule 1 Must be at least 1 token
        if (formula == String.Empty || Regex.IsMatch(formula, @"^\s+$"))
            throw new FormulaFormatException("Empty formula");

        valueStack = new Stack<string>();
        operatorStack = new Stack<string>();

        _formulaTokens = GetTokens(formula);

        IsValidFormula(_formulaTokens);

        _formulaString = BuildString(_formulaTokens);
    }

    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    ///   <remarks>
    ///     Important: no variable may appear more than once in the returned set, even
    ///     if it is used more than once in the Formula.
    ///     Variables should be returned in canonical form, having all letters converted
    ///     to uppercase.
    ///   </remarks>
    ///   <list type="bullet">
    ///     <item>new("x1+y1*z1").GetVariables() should return a set containing "X1", "Y1", and "Z1".</item>
    ///     <item>new("x1+X1"   ).GetVariables() should return a set containing "X1".</item>
    ///   </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables()
    {
        HashSet<string> formulaVariables = [];
        foreach (string token in _formulaTokens)

        {
            if (IsVar(token) && !formulaVariables.Contains(token))
                formulaVariables.Add(token.ToUpper());
        }

        return formulaVariables;
    }

    /// <summary>
    ///   <para>
    ///     Returns a string representation of a canonical form of the formula.
    ///   </para>
    ///   <para>
    ///     The string will contain no spaces.
    ///   </para>
    ///   <para>
    ///     If the string is passed to the Formula constructor, the new Formula f
    ///     will be such that this.ToString() == f.ToString().
    ///   </para>
    ///   <para>
    ///     All the variable and number tokens in the string will be normalized.
    ///     For numbers, this means that the original string token is converted to
    ///     a number using double.Parse or double.TryParse, then converted back to a
    ///     string using double.ToString.
    ///     For variables, this means all letters are uppercased.
    ///   </para>
    ///   <para>
    ///       For example:
    ///   </para>
    ///   <code>
    ///       new("x1 + Y1").ToString() should return "X1+Y1"
    ///       new("x1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    ///   <para>
    ///     This method should execute in O(1) time.
    ///   </para>
    /// </summary>
    /// <returns>
    ///   A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString()
    {
        return _formulaString;
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string token)
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }


    private static bool IsNumber(string token)
    {
        return double.TryParse(token, out _);
    }

    /// <summary>
    /// Checks if there is an equal number of opening and closing parentheses. Opening parentheses add 1
    /// to the count and closing parentheses subtract 1.
    /// </summary>
    /// <param name="formula">List of tokens</param>
    /// <returns>True if the count is 0 and false otherwise</returns>
    private static bool ParenthesesCheck(List<string> formula)
    {
        int paraBalance = 0;
        foreach (string token in _formulaTokens)
        {
            if (Regex.IsMatch(token, @"\("))
                paraBalance++;
            else if (Regex.IsMatch(token, @"\)"))
                paraBalance--;
        }

        return paraBalance == 0;
    }

    /// <summary>
    /// Checks if a token is valid. Token must be a number, variable, parentheses or operator.
    /// </summary>
    /// <param name="tokens">List of tokens</param>
    /// <returns>True if token is a valid token and throws a Formula Format Exception if not</returns>
    /// <exception cref="FormulaFormatException"></exception>
    private static bool IsValidToken(List<string> tokens)
    {
        if (tokens.Count == 1)
            return SingleTokenValidity(tokens[0]);

        if (!Regex.IsMatch(tokens[0], FirstTokenRegExPattern))
            throw new FormulaFormatException($"Invalid first token");

        bool validToken = true;

        foreach (string token in tokens)
        {
            if (IsNumber(token))
                validToken = true;

            else if (IsVar(token))
                validToken = true;

            else if (Regex.IsMatch(token, @"^[\(\)\+\-*/]$"))
                validToken = true;

            else
                validToken = false;
        }

        if (validToken)
            BuildString(tokens);

        return validToken;
    }

    private static string BuildString(List<string> tokens)
    {
        string newFormulaString = "";

        foreach (string token in tokens)
        {
            if (IsNumber(token))
                newFormulaString += double.Parse(token);

            else if (IsVar(token))
                newFormulaString += token.ToUpper();

            else if (Regex.IsMatch(token, @"^[\(\)\+\-*/]$"))
                newFormulaString += token;
        }


        return newFormulaString;
    }

    /// <summary>
    /// Checks if the token following a parenthesis or operator is a valid following token.
    /// Must be a number, variable, or an opening parenthesis if token is an opening paranthesis.
    /// If token is an operator, the next token must be a number, variable, or opening parenthesis.
    /// </summary>
    /// <param name="tokens">List of tokens</param>
    /// <returns>True if the token following a parenthesis or operator is not one of the valid options</returns>
    private static bool IsValidParaOperFollowing(List<string> tokens)
    {
        string lpPattern = @"^\($";
        string opPattern = @"^[\+\-*/]$";

        for (int i = 0; i < tokens.Count - 1; i++)
        {
            string current = tokens[i];
            string next = tokens[i + 1];

            if (Regex.IsMatch(current, lpPattern) || Regex.IsMatch(current, opPattern))
            {
                if (!IsNumber(next) && !IsVar(next) && !Regex.IsMatch(next, lpPattern))
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if the token following a closing parenthesis valid token. The token
    /// after a closing parenthesis must be either another closing parenthesis or an operator.
    /// </summary>
    /// <param name="tokens">List of tokens</param>
    /// <returns>False if the token following a closing parenthesis is not another closing parenthesis
    /// or operator. True if the next token is neither of those. </returns>
    private static bool IsValidExtraFollowing(List<string> tokens)
    {
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";

        for (int i = 0; i < tokens.Count - 1; i++)
        {
            string current = tokens[i];

            if (i == tokens.Count - 1)
                return Regex.IsMatch(current, LastTokenRegExPattern);

            string next = tokens[i + 1];

            if (Regex.IsMatch(current, rpPattern))
            {
                if (!Regex.IsMatch(next, rpPattern) &&
                    !Regex.IsMatch(next, opPattern))
                    return false;
            }
            else if (Regex.IsMatch(current, opPattern))
            {
                if (!IsNumber(next) && !IsVar(next) && !Regex.IsMatch(next, @"\("))
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks to see if a single token is a number or variable.
    /// </summary>
    /// <param name="token">String that represents a token to be checked</param>
    /// <returns>True if the token is a valid number or variable</returns>
    private static bool SingleTokenValidity(string token)
    {
        if (Regex.IsMatch(token, @"\s"))
            throw new FormulaFormatException($"Invalid token");

        if (!IsNumber(token) && !IsVar(token))
            return false;


        return true;
    }

    /// <summary>
    /// Helper method to ensure none of the rules are broken when a formula is passed through. Ensures token is valid
    /// first. Makes sure that there are an equal number of parenthesis. Uses parentheses/operator
    /// following token is valid. Uses helper method to ensure the token after a closing parenthesis is valid.
    /// Checks last token to make sure it is a closing parenthesis, number, or variable.
    /// </summary>
    /// <param name="formula">List of tokens that represents the formula being checked</param>
    /// <exception cref="FormulaFormatException"></exception>
    private static void IsValidFormula(List<string> formula)
    {
        // Rule 2 Valid Tokens
        IsValidToken(formula);

        // Rule 3 & 4, Closing and Balanced Parentheses
        if (!ParenthesesCheck(formula))
            throw new FormulaFormatException($"Invalid Parentheses amount");

        // Rule 7, Parentheses/Operator following
        if (!IsValidParaOperFollowing(formula))
            throw new FormulaFormatException($"Invalid token following Parantheses or Operator");

        // Rule 8, Extra Following
        if (!IsValidExtraFollowing(formula))
            throw new FormulaFormatException($"Invalid Extra following");

        // Checks that the last token is a ), number, or variable
        if (!Regex.IsMatch(formula[^1], LastTokenRegExPattern))
            throw new FormulaFormatException($"Invalid last token");
    }


    /// <summary>
    ///   <para>
    ///     Given an expression, enumerates the tokens that compose it.
    ///   </para>
    ///   <para>
    ///     Tokens returned are:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>left paren</item>
    ///     <item>right paren</item>
    ///     <item>one of the four operator symbols</item>
    ///     <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///     <item>a double literal</item>
    ///     <item>and anything that doesn't match one of the above patterns</item>
    ///   </list>
    ///   <para>
    ///     There are no empty tokens; white space is ignored (except to separate other tokens).
    ///   </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens(string formula)
    {
        List<string> results = [];

        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format(
            "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
            lpPattern,
            rpPattern,
            opPattern,
            VariableRegExPattern,
            doublePattern,
            spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                results.Add(s);
            }
        }

        return results;
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 == f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are the same.</returns>
    public static bool operator ==(Formula f1, Formula f2)
    {
        return f1.Equals(f2);
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 != f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are not equal to each other.</returns>
    public static bool operator !=(Formula f1, Formula f2)
    {
        return !f1.Equals(f2);
    }

    /// <summary>
    ///   <para>
    ///     Determines if two formula objects represent the same formula.
    ///   </para>
    ///   <para>
    ///     By definition, if the parameter is null or does not reference
    ///     a Formula Object then return false.
    ///   </para>
    ///   <para>
    ///     Two Formulas are considered equal if their canonical string representations
    ///     (as defined by ToString) are equal.
    ///   </para>
    /// </summary>
    /// <param name="obj"> The other object.</param>
    /// <returns>
    ///   True if the two objects represent the same formula.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj == null || !(obj is Formula))
            return false;

        Formula other = (Formula)obj;

        return Equals(ToString(), other.ToString());
    }

    /// <summary>
    ///   <para>
    ///     Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    ///     case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    ///     randomly-generated unequal Formulas have the same hash code should be miniscule.
    ///   </para>
    /// </summary>
    /// <returns> The hashcode for the object. </returns>
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    /// <summary>
    ///   <para>
    ///     Evaluates this Formula, using the lookup delegate to determine the values of
    ///     variables.
    ///   </para>
    ///   <remarks>
    ///     When the lookup method is called, it will always be passed a normalized (capitalized)
    ///     variable name.  The lookup method will throw an ArgumentException if there is
    ///     not a definition for that variable token.
    ///   </remarks>
    ///   <para>
    ///     If no undefined variables or divisions by zero are encountered when evaluating
    ///     this Formula, the numeric value of the formula is returned.  Otherwise, a
    ///     FormulaError is returned (with a meaningful explanation as the Reason property).
    ///   </para>
    ///   <para>
    ///     This method should never throw an exception.
    ///   </para>
    /// </summary>
    /// <param name="lookup">
    ///   <para>
    ///     Given a variable symbol as its parameter, lookup returns the variable's value
    ///     (if it has one) or throws an ArgumentException (otherwise).  This method will expect
    ///     variable names to be normalized.
    ///   </para>
    /// </param>
    /// <returns> Either a double or a FormulaError, based on evaluating the formula.</returns>
    public object Evaluate(Lookup lookup)
    {
        foreach (string token in _formulaTokens)
        {
            if (IsNumber(token) || IsVar(token))
            {
                double right;
                
                if (IsNumber(token))
                    right = double.Parse(token);
                else
                    right = lookup(token);
                if (operatorStack.Count > 0  && Regex.IsMatch(operatorStack.Peek(), @"[\*/]"))
                {
                    MultiplyOrDivide(right);
                }
                else
                    valueStack.Push(right.ToString());
            }
            else if (Regex.IsMatch(token, @"[\+-]"))
            {
                if (operatorStack.Count > 0)
                {
                    AddOrSubtract();
                    operatorStack.Push(token);
                }
                else
                {
                    operatorStack.Push(token);
                }
            }
            else if (Regex.IsMatch(token, @"[\*/(]"))
            {
                operatorStack.Push(token);
            }
            else if (Regex.IsMatch(token, @"\)"))
            {
                if (operatorStack.Count > 0)
                {
                    string op = operatorStack.Peek();

                    if (Regex.IsMatch(op, @"[\+-]"))
                    {
                        AddOrSubtract();
                    }
                    else if (Regex.IsMatch(op, @"[\*/]"))
                    {
                        double right = double.Parse(valueStack.Pop());
                        MultiplyOrDivide(right);
                    }
                    operatorStack.Pop();
                }
            }
        }

        if (operatorStack.Count > 0)
        {
            AddOrSubtract();
        }
        
        return double.Parse(valueStack.Pop());

    }

    private void AddOrSubtract()
    {
        string op = operatorStack.Peek();
        
        if (Regex.IsMatch(op, @"\+") || Regex.IsMatch(op, "-"))
        {
            double right = double.Parse(valueStack.Pop());
            double result = double.Parse(valueStack.Pop());
            operatorStack.Pop();
            if (Regex.IsMatch(op, @"\+"))
            {
                result += right;
            }
            else
            {
                result -= right;
            }

            valueStack.Push(result.ToString());
        }

    }

    private void MultiplyOrDivide(double right)
    {
        string op = operatorStack.Peek();

        if (Regex.IsMatch(op, @"\*") || Regex.IsMatch(op, "/"))
        {
            double result = double.Parse(valueStack.Pop());

            operatorStack.Pop();

            if (Regex.IsMatch(op, @"\*"))
            {
                result *= right;
            }
            else
            {
                if (result == 0)
                {
                    new FormulaError("Cannot divide by 0");
                }
                else
                    result /= right;
            }

            valueStack.Push(result.ToString());
        }
    }
}

/// <summary>
///   Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///   <para>
    ///      Constructs a FormulaFormatException containing the explanatory message.
    ///   </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException(string message)
        : base(message)
    {
        // All this does is call the base constructor. No extra code needed.
    }
}

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public class FormulaError
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaError"/> class.
    ///   <para>
    ///     Constructs a FormulaError containing the explanatory reason.
    ///   </para>
    /// </summary>
    /// <param name="message"> Contains a message for why the error occurred.</param>
    public FormulaError(string message)
    {
        Reason = message;
    }

    /// <summary>
    ///  Gets the reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}

/// <summary>
///   Any method meeting this type signature can be used for
///   looking up the value of a variable.
/// </summary>
/// <exception cref="ArgumentException">
///   If a variable name is provided that is not recognized by the implementing method,
///   then the method should throw an ArgumentException.
/// </exception>
/// <param name="variableName">
///   The name of the variable (e.g., "A1") to lookup.
/// </param>
/// <returns> The value of the given variable (if one exists). </returns>
public delegate double Lookup(string variableName);