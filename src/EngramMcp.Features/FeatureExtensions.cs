using EngramMcp.Core;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace EngramMcp.Features;

public static class FeatureExtensions
{
    extension(MemoryContainer container)
    {
        internal string ToRecallMarkdown()
        {
            var sb = new StringBuilder("# Memory");

            foreach (var block in container.Memories)
            {
                sb.AppendLine().AppendLine($"## {block.Key}");

                foreach (var memory in block.Value)
                    sb.AppendLine(FormatMemoryLine(memory));
            }

            if (container.CustomSections.Count > 0)
            {
                sb.AppendLine().AppendLine("## Custom Sections");

                foreach (var section in container.CustomSections
                    .OrderByDescending(summary => summary.EntryCount)
                    .ThenBy(summary => summary.Name, StringComparer.Ordinal))
                    sb.AppendLine($"- {section.Name} ({section.EntryCount})");
            }

            return sb.ToString();
        }

        internal string ToSectionMarkdown()
        {
            var sb = new StringBuilder("# Memory");

            foreach (var block in container.Memories)
            {
                sb.AppendLine().AppendLine($"## {block.Key}");

                foreach (var memory in block.Value)
                    sb.AppendLine(FormatMemoryLine(memory));
            }

            return sb.ToString();
        }
    }

    extension(IReadOnlyList<MemorySearchResult> results)
    {
        internal string ToMarkdown()
        {
            var sb = new StringBuilder("# Memory Search Results");

            if (results.Count == 0)
                return sb.AppendLine().AppendLine("No matches found.").ToString();

            sb.AppendLine();

            foreach (var result in results)
                sb.AppendLine(FormatSearchLine(result));

            return sb.ToString();
        }
    }

    private static string FormatMemoryLine(MemoryEntry memory)
    {
        return $"- {memory.Text}{FormatImportanceSuffix(memory.Importance)}{FormatTagsSuffix(memory.Tags)}";
    }

    private static string FormatSearchLine(MemorySearchResult result)
    {
        return $"- {result.Entry.Text}{FormatImportanceSuffix(result.Entry.Importance)} (`{result.Section}`){FormatTagsSuffix(result.Entry.Tags)}";
    }

    private static string FormatImportanceSuffix(MemoryImportance importance)
    {
        return importance == MemoryImportance.High ? " - IMPORTANT!" : string.Empty;
    }

    private static string FormatTagsSuffix(IReadOnlyList<string> tags)
    {
        return tags.Count == 0 ? string.Empty : $" [tags: {string.Join(", ", tags)}]";
    }

    public static IEnumerable<Type> GetImplementations<T>() => Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(type => type.Implements<T>())
        .Distinct();

    extension(IServiceCollection services)
    {
        public IServiceCollection AddImplementations<T>()
        {
            foreach (var type in GetImplementations<T>())
                services.AddSingleton(type);

            return services;
        }
    }

    extension(Type type)
    {
        private bool Implements<T>() => type is { IsClass: true, IsAbstract: false } && type.IsAssignableTo(typeof(T));
    }
}
