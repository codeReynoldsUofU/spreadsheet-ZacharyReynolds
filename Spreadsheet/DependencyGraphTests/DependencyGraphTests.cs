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
    /// This test class is to ensure that our Add/Remove Dependency and Get Dependents/Dependees methods are O(1) time.  
    /// Creates a collection of strings, then creates another two sets, one to represent dependents and one to represent
    /// dependents. Dependencies are added to the Dependency graph, then every fourth one is removed. After the removal,
    /// more dependencies are added back and then every third dependency is removed. Lastly, it reports that all
    /// dependency addition and removal were performed correctly
    /// </summary>
    [TestMethod]
    [Timeout(2000)] // 2 second run time limit
    public void StressTest()
    {
        DependencyGraph dg = new();
// Creates a bunch a list of 200 strings
        const int SIZE = 200;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }

// Collection of nodes to be tested against later to ensure proper code implementation
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
    
    /// <summary>
    /// Correctly reports that node "A1" does have dependents
    /// </summary>
    [TestMethod]
    public void HasDependentsTest_True()
    {
        DependencyGraph dg = new();
        dg.AddDependency("A1", "A2");
        dg.AddDependency("A1", "A3");
        dg.AddDependency("A1", "A4");
        Assert.IsTrue(dg.HasDependents("A1"));
    }
    
    /// <summary>
    /// Correctly reports that cell "A2" does not have any dependents after adding dependencies to "A1"
    /// </summary>
    [TestMethod]
    public void HasDependentsTest_False()
    {
        DependencyGraph dg = new();
        dg.AddDependency("A1", "A2");
        dg.AddDependency("A1", "A3");
        dg.AddDependency("A1", "A4");
        Assert.IsFalse(dg.HasDependents("A2"));
    }

    /// <summary>
    /// Correctly reports that node "A2" is a dependent to any cell
    /// </summary>
    [TestMethod]
    public void HasDependeesTest_True()
    { 
        DependencyGraph dg = new();
        dg.AddDependency("A1", "A2");
        dg.AddDependency("A3", "A2"); 
        dg.AddDependency("A3", "A2");
        
        Assert.IsTrue(dg.HasDependees("A2"));
    }

    /// <summary>
    /// Correctly reports that node "A1" is not dependent to any cells
    /// </summary>
    [TestMethod]
    public void HasDependeesTest_False()
    {
        DependencyGraph dg = new();
        dg.AddDependency("A1", "A2");
        dg.AddDependency("A3", "A2"); 
        dg.AddDependency("A3", "A2");
        
        Assert.IsFalse(dg.HasDependees("A1"));
    }
    
    /// <summary>
    /// Gets a collection of dependents of cell "A1"
    /// </summary>
    [TestMethod]
    public void GetDependentsTest_HasDependencies()
    {
        DependencyGraph dg = new();
        dg.AddDependency("A1", "A2");
        dg.AddDependency("A1", "A3");
        dg.AddDependency("A1", "A4");
        dg.AddDependency("A1", "A4");
        HashSet<string> dependents = new HashSet<string>{"A2", "A3", "A4"};
        
        // Correctly reports that "A1" has the same dependents as the created HashSet
        Assert.IsTrue(dependents.SetEquals(dg.GetDependents("A1")));
    }

    /// <summary>
    /// Shows that an empty collection is returned when GetDependents is called on a node
    /// that has no dependencies
    /// </summary>
    [TestMethod]
    public void GetDependentsTest_NoDependents()
    {
        DependencyGraph dg = new();
        Assert.IsEmpty(dg.GetDependents("A1"));
    }
    
    /// <summary>
    /// Gets a collection of dependees for cell "A2"
    /// </summary>
    [TestMethod]
    public void GetDependeesTest_HasDependencies()
    {
        DependencyGraph dg = new();
        const int SIZE = 20;
        HashSet<string> dependees = new HashSet<string>();
        dg.AddDependency("A1", "A2");
        
        // Adds dependencies to DependencyGraph and nodes to a HashSet that represents dependees
        for (int i = 1; i < SIZE; i++)
        {
            dependees.Add($"A{i}");
            dg.AddDependency($"A{i}", "A2");
        }
        
        // Shows that our created HashSet and the HashSet returned from GetDependees is the same
        Assert.IsTrue(dependees.SetEquals(dg.GetDependees("A2")));
    }

    /// <summary>
    /// Shows that an empty collection is returned when GetDependees is called on a node
    /// that has no dependencies
    /// </summary>
    [TestMethod]
    public void GetDependeesTest_NoDependees()
    {
        DependencyGraph dg = new();
        Assert.IsEmpty(dg.GetDependees("A1"));
    }
    
    /// <summary>
    /// Creates a DependencyGraph and adds dependencies, then removes certain ones and shows that they were
    /// correctly removed
    /// </summary>
    [TestMethod]
    public void RemoveDependecyTest()
    {
        DependencyGraph dg = new();
        const int SIZE = 20;
        HashSet<string> dependees = new HashSet<string>();
        dg.AddDependency("A1", "A2");
        
        // Adds dependencies to DependencyGraph and nodes to a HashSet that represents dependees
        for (int i = 1; i < SIZE; i++)
        {
            dependees.Add($"A{i}");
            dg.AddDependency($"A{i}", "A2");
        }
        
        // Removes every other dependency from A2
        for (int i = 1; i < SIZE; i+=2)
        {
            dependees.Remove($"A{i}");
            dg.RemoveDependency($"A{i}", "A2");
        }
        
        // Shows that our created HashSet and the HashSet returned from GetDependees is the same
        Assert.IsTrue(dependees.SetEquals(dg.GetDependees("A2")));
    }
    
    /// <summary>
    /// Test the ReplaceDependents method, showing that the dependents for a node are properly swapped
    /// with a given set
    /// </summary>
    [TestMethod]
    public void ReplaceDependents()
    {
        DependencyGraph dg = new();
        const int SIZE = 20;
        dg.AddDependency("A1", "A2");
        HashSet<string> dependents = new HashSet<string>();
        
        // Adds nodes to our created HashSet
        for (int i = 3; i < SIZE; i++)
        {
            dependents.Add($"A{i}");
        }
        
        // Replaces the one dependency added to the DependencyGraph with the collection of 
        // nodes 
        dg.ReplaceDependents("A1", dependents);
        
        // Reports that the list of nodes we created was properly swapped with "A1" dependents
        Assert.IsTrue(dependents.SetEquals(dg.GetDependents("A1")));
    }
    
    /// <summary>
    /// Test the ReplaceDependees method, showing that the dependees for a node are properly swapped
    /// with a given set
    /// </summary>
    [TestMethod]
    public void ReplaceDependees()
    {
        DependencyGraph dg = new();
        const int SIZE = 20;
        HashSet<string> dependees = new HashSet<string>();
        dg.AddDependency("A1", "A2");
        
        // Adds nodes to our created HashSet
        for (int i = 3; i < SIZE; i++)
        {
            dependees.Add($"A{i}");
            dg.AddDependency($"A{i}", "A2");
        }
        
        // Replaces "A2" dependees with collection of nodes
        dg.ReplaceDependees("A2", dependees);
        
        // Reports that "A2" dependees were correctly swapped with the given collection of nodes
        Assert.IsTrue(dependees.SetEquals(dg.GetDependees("A2")));
    }
}