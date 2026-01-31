namespace DependencyGraphTests;

using DependencyGraph;

/// <summary>
/// This is a test class for DependencyGraphTest and is intended
/// to contain all DependencyGraphTest Unit Tests
/// </summary>
[TestClass]
public class DependencyGraphTests
{
    /// <summary>
    /// TODO: Explain carefully what this code tests.
    /// Also, update in-line comments as appropriate.
    /// </summary>
    [TestMethod]
    [Timeout(2000)] // 2 second run time limit
    public void StressTest()
    {
        DependencyGraph dg = new();
// A bunch of strings to use
        const int SIZE = 200;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }

// The correct answers
        HashSet<string>[] dependents = new HashSet<string>[SIZE];
        HashSet<string>[] dependees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dependents[i] = [];
            dependees[i] = [];
        }

// Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

// Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

// Add some back
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

// Remove some more
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = i + 3; j < SIZE; j += 3)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

// Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dependents[i].SetEquals(new
                HashSet<string>(dg.GetDependents(letters[i]))));
            Assert.IsTrue(dependees[i].SetEquals(new
                HashSet<string>(dg.GetDependees(letters[i]))));
        }
    }
    
    [TestMethod]
    public void HasDependentsTest_True()
    {
        DependencyGraph dg = new();
        dg.AddDependency("A1", "A2");
        dg.AddDependency("A1", "A3");
        dg.AddDependency("A1", "A4");
        Assert.IsTrue(dg.HasDependents("A1"));
    }
    
    [TestMethod]
    public void HasDependentsTest_False()
    {
        DependencyGraph dg = new();
        dg.AddDependency("A1", "A2");
        dg.AddDependency("A1", "A3");
        dg.AddDependency("A1", "A4");
        Assert.IsFalse(dg.HasDependents("A2"));
    }
    
    [TestMethod]
    public void GetDependentsTest_Valid()
    {
        DependencyGraph dg = new();
        dg.AddDependency("A1", "A2");
        dg.AddDependency("A1", "A3");
        dg.AddDependency("A1", "A4");
        dg.AddDependency("A1", "A4");
        HashSet<string> dependents = new HashSet<string>{"A2", "A3", "A4"};
        Assert.IsTrue(dependents.SetEquals(dg.GetDependents("A1")));
    }
    [TestMethod]
    public void GetDependeesTest_Valid()
    {
        DependencyGraph dg = new();
        const int SIZE = 20;
        HashSet<string> dependees = new HashSet<string>();
        dg.AddDependency("A1", "A2");
        
        for (int i = 1; i < SIZE; i++)
        {
            dependees.Add($"A{i}");
            dg.AddDependency($"A{i}", "A2");
        }
        Assert.IsTrue(dependees.SetEquals(dg.GetDependees("A2")));
    }
    [TestMethod]
    public void RemoveDependecyTest()
    {
        DependencyGraph dg = new();
        const int SIZE = 20;
        HashSet<string> dependees = new HashSet<string>();
        dg.AddDependency("A1", "A2");
        
        for (int i = 1; i < SIZE; i++)
        {
            dependees.Add($"A{i}");
            dg.AddDependency($"A{i}", "A2");
        }

        for (int i = 1; i < SIZE; i+=2)
        {
            dependees.Remove($"A{i}");
            dg.RemoveDependency($"A{i}", "A2");
        }
        Assert.IsTrue(dependees.SetEquals(dg.GetDependees("A2")));
    }
    [TestMethod]
    public void ReplaceDependents()
    {
        DependencyGraph dg = new();
        const int SIZE = 20;
        dg.AddDependency("A1", "A2");
        HashSet<string> dependents = new HashSet<string>();
        for (int i = 3; i < SIZE; i++)
        {
            dependents.Add($"A{i}");
        }
        dg.ReplaceDependents("A1", dependents);
        Assert.IsTrue(dependents.SetEquals(dg.GetDependents("A1")));
    }
    
    [TestMethod]
    public void ReplaceDependees()
    {
        DependencyGraph dg = new();
        const int SIZE = 20;
        HashSet<string> dependees = new HashSet<string>();
        dg.AddDependency("A1", "A2");
        for (int i = 3; i < SIZE; i++)
        {
            dependees.Add($"A{i}");
            dg.AddDependency($"A{i}", "A2");
        }
        dg.ReplaceDependees("A2", dependees);
        Assert.IsTrue(dependees.SetEquals(dg.GetDependees("A2")));
    }
}