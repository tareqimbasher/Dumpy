using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dumpy.Utils;

namespace Dumpy.Metadata;

public class DataMembers
{
    private static readonly DataMembers _empty = new DataMembers();
    
    private DataMembers()
    {
        Properties = Array.Empty<PropertyInfo>();
        Fields = Array.Empty<FieldInfo>();
    }
    
    public DataMembers(IList<PropertyInfo> properties, IList<FieldInfo> fields)
    {
        Properties = properties.ToArray();
        Fields = fields.ToArray();
    }

    public IReadOnlyCollection<PropertyInfo> Properties { get; }
    public IReadOnlyCollection<FieldInfo> Fields { get; }
    
    public bool IsEmpty => Properties.Count == 0 && Fields.Count == 0;

    public static DataMembers Emtpy => _empty;
}
