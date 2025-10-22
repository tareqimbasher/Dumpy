using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Dumpy.Utils;

internal sealed class TypeMemberCache
{
    private readonly ConcurrentDictionary<Type, CacheEntry> _types = new(ReferenceEqualityComparer.Instance);

    public Members GetMembers(Type type, bool includeNonPublic)
    {
        var entry = _types.GetOrAdd(type, static _ => new CacheEntry());
        return includeNonPublic ? entry.NonPublicIncludedMembers : entry.PublicOnlyMembers;
    }

    private sealed class CacheEntry
    {
        private Members? _publicOnlyMembers;
        private Members? _nonPublicIncludedMembers;
        
        public Members PublicOnlyMembers => _publicOnlyMembers ??= new Members();
        public Members NonPublicIncludedMembers => _nonPublicIncludedMembers ??= new Members();
    }

    internal sealed class Members
    {
        public PropertyInfo[]? Properties { get; set; }
        public FieldInfo[]? Fields { get; set; }
    }
}