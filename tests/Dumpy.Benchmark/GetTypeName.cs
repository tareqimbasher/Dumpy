using System.Text;
using BenchmarkDotNet.Attributes;

namespace Dumpy.Benchmark;

[MemoryDiagnoser]
public class GetTypeName
{
    [Params(
        // typeof(int),
        // typeof(IEnumerable<int>),
        // typeof(Dictionary<int, int>),
        // typeof(ValueTuple<int, int, int>),
        // typeof(ValueTuple<int, int, int, int>),
        //typeof(ValueTuple<int, int, int, int, int>),
        typeof(ValueTuple<int, int, int, int, int, int>)
    )]
    public Type Type { get; set; }

    [ParamsAllValues] public bool FullyQualify;

    // [Benchmark]
    // public string OldWay() => GetNameOldWay(Type, FullyQualify);

    [Benchmark]
    public string NewWay() => GetFullName(Type, FullyQualify);

    static string GetFullName(Type type, bool fullName)
    {
        var name = fullName ? type.FullName ?? type.Name : type.Name;

        if (!type.IsGenericType)
        {
            return name;
        }

        var sb = new StringBuilder();

        if (type.Namespace == null && name.Contains("AnonymousType"))
        {
            sb.Append("AnonymousType");
        }
        else
        {
            sb.Append(name.AsSpan(0, name.IndexOf('`')));
        }

        sb.Append(
            type
                .GetGenericArguments()
                .Aggregate("<",
                    delegate(string aggregate, Type argType)
                    {
                        return aggregate + (aggregate == "<" ? "" : ",") + GetFullName(argType, fullName);
                    }
                ));
        sb.Append('>');

        return sb.ToString();
    }

    private static string GetNameOldWay(Type type, bool fullyQualify = false)
    {
        string name = type.FullName ?? type.Name;

        if (type.GenericTypeArguments.Length > 0)
        {
            name = name.Split('`')[0];
            name += "<";

            foreach (var tArg in type.GenericTypeArguments)
            {
                name += GetNameOldWay(tArg, fullyQualify) + ", ";
            }

            name = name.TrimEnd(' ', ',') + ">";
        }

        if (!fullyQualify)
        {
            string typeNamespace = type.Namespace ?? string.Empty;

            if (typeNamespace.Length > 1 && name.StartsWith(typeNamespace))
                name = name.Substring(typeNamespace.Length + 1); // +1 to trim the '.' after the namespace
        }

        if (type.FullName?.StartsWith("System.Nullable`") == true)
        {
            int iStart = fullyQualify ? "System.Nullable<".Length : "Nullable<".Length;
            name = name.Substring(iStart, name.Length - iStart) + "?";
        }

        return name;
    }

    private static string GetNameNewWay(Type type, bool fullyQualify = false)
    {
        if (type.GenericTypeArguments.Length == 0)
        {
            return fullyQualify ? type.FullName ?? type.Name : type.Name;
        }

        var builder = new ValueStringBuilder(stackalloc char[128], 128);

        try
        {
            GetNameNewWayInner(ref builder, type, fullyQualify);
            return builder.ToString();
        }
        catch
        {
            builder.Dispose();
            throw;
        }
    }

    private static void GetNameNewWayInner(ref ValueStringBuilder builder, Type type, bool fullyQualify = false)
    {
        if (type.GenericTypeArguments.Length == 0)
        {
            builder.Append(fullyQualify ? type.FullName ?? type.Name : type.Name);
            return;
        }

        // Special case for Nullable<> when outputting non-fully-qualified name
        if (!fullyQualify)
        {
            var nullableUnderlyingType = Nullable.GetUnderlyingType(type);

            if (nullableUnderlyingType != null)
            {
                if (nullableUnderlyingType.GenericTypeArguments.Length == 0)
                {
                    builder.Append(type.Name);
                }
                else
                {
                    GetNameNewWayInner(ref builder, nullableUnderlyingType, fullyQualify);
                }

                builder.Append('?');
                return;
            }
        }

        var name = fullyQualify ? type.FullName ?? type.Name : type.Name;

        builder.Append(name.Substring(name.IndexOf('`')));
        builder.Append('<');

        for (var index = 0; index < type.GenericTypeArguments.Length; index++)
        {
            var gta = type.GenericTypeArguments[index];
            GetNameNewWayInner(ref builder, gta, fullyQualify);

            if (index != type.GenericTypeArguments.Length - 1)
            {
                builder.Append(", ");
            }
        }

        builder.Append('>');
    }
}