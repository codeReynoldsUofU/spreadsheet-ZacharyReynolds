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
[Timeout( 2000 )] // 2 second run time limit
public void StressTest()
{
DependencyGraph dg = new();
// A bunch of strings to use
const int SIZE = 200;
string[] letters = new string[SIZE];
for ( int i = 0; i < SIZE; i++ )
{
letters[i] = string.Empty + ( (char) ( 'a' + i ) );
}
// The correct answers
HashSet<string>[] dependents = new HashSet<string>[SIZE];
HashSet<string>[] dependees = new HashSet<string>[SIZE];
for ( int i = 0; i < SIZE; i++ )
{
dependents[i] = [];
dependees[i] = [];
}
// Add a bunch of dependencies
for ( int i = 0; i < SIZE; i++ )
{
for ( int j = i + 1; j < SIZE; j++ )
{
dg.AddDependency( letters[i], letters[j] );
dependents[i].Add( letters[j] );
dependees[j].Add( letters[i] );
}
}
// Remove a bunch of dependencies
for ( int i = 0; i < SIZE; i++ )
{
for ( int j = i + 4; j < SIZE; j += 4 )
{
dg.RemoveDependency( letters[i], letters[j] );
dependents[i].Remove( letters[j] );
dependees[j].Remove( letters[i] );
}
}
// Add some back
for ( int i = 0; i < SIZE; i++ )
{
for ( int j = i + 1; j < SIZE; j += 2 )
{
dg.AddDependency( letters[i], letters[j] );
dependents[i].Add( letters[j] );
dependees[j].Add( letters[i] );
}
}
// Remove some more
for ( int i = 0; i < SIZE; i += 2 )
{
for ( int j = i + 3; j < SIZE; j += 3 )
{
dg.RemoveDependency( letters[i], letters[j] );
dependents[i].Remove( letters[j] );
dependees[j].Remove( letters[i] );
}
}
// Make sure everything is right
for ( int i = 0; i < SIZE; i++ )
{
Assert.IsTrue( dependents[i].SetEquals( new
HashSet<string>( dg.GetDependents( letters[i] ) ) ) );
Assert.IsTrue( dependees[i].SetEquals( new
HashSet<string>( dg.GetDependees( letters[i] ) ) ) );
}
}
}
