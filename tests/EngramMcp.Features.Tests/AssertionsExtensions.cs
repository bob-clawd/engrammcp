using Is.Assertions;
namespace EngramMcp.Features.Tests;

public static class AssertionsExtensions
{
    extension(string actualPath)
    {
        internal bool HasPathSuffix(string expectedPathSuffix)
        {
            return NormalizePathSeparators(actualPath).EndsWith(NormalizePathSeparators(expectedPathSuffix), StringComparison.OrdinalIgnoreCase);
        }

        internal void ShouldEndWithPathSuffix(string expectedPathSuffix)
        {
            actualPath.HasPathSuffix(expectedPathSuffix).IsTrue();
        }
    }

    internal static void ShouldNotBeEmpty(this string text)
    {
        string.IsNullOrEmpty(text).IsFalse();
    }
    
    private static string NormalizePathSeparators(string path)
    {
        return path.Replace('/', '\\');
    }
}
