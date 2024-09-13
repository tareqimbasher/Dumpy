using System;
using System.Reflection;

namespace Dumpy.Metadata;

public interface ITypeMetadataProvider
{
    string GetName(Type type, bool fullyQualify = false);
    DataMembers GetDataMembers(Type type);
}