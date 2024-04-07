namespace Codex.Dtos
{
    internal class StringInfoTypeDto : IEquatable<StringInfoTypeDto>
    {
        public StringInfoTypeDto(string? typeName, string? typeNamespace)
        {
            TypeName = typeName ?? string.Empty;
            TypeNamespace = typeNamespace ?? string.Empty;
        }

        public string TypeName { get; init; }

        public string TypeNamespace { get; init; }

        public bool Equals(StringInfoTypeDto? other)
        {
            if (other is null)
                return false;

            return TypeName == other.TypeName && TypeNamespace == other.TypeNamespace;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not StringInfoTypeDto other)
                return false;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return (TypeName?.GetHashCode() ?? 0) ^ (TypeNamespace?.GetHashCode() ?? 0);
        }
    }
}
