using Formula;

namespace FormulaTests;

[TestClass]
public sealed class FormulaValidity
{
    [TestMethod]
    public void TestMethod1()
    {
        Assert.Throws<FormulaFormatException>( () => _ = new Formula.Formula("1+1") );
    }
}