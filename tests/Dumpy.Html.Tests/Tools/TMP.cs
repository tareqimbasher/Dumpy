using System.Text;
using Xunit.Abstractions;

namespace Dumpy.Html.Tests.Tools;

public class TMP(ITestOutputHelper testOutputHelper)
{
    [Fact(Skip = "")]
    public void Test()
    {
        // var sb = new StringBuilder();
        // var sw = new StringWriter(sb);
        // Console.SetOut(sw);
        //
        // //var builder = new ValueStringBuilder(100);
        // //builder.Append(typeof(ValueTuple<int, int, int, int, int, int>).FullName);
        // //_ = builder.ToString();
        //
        // GetNameNewWay(typeof(ValueTuple<int, int, int, int>), true);
        //
        // testOutputHelper.WriteLine(sb.ToString());

        testOutputHelper.WriteLine("1. " + GetFullName(typeof(int), false));
        testOutputHelper.WriteLine("2. " + GetFullName(typeof(int), true));
        testOutputHelper.WriteLine("3. " + GetFullName(typeof(Nullable<int>), false));
        testOutputHelper.WriteLine("4. " + GetFullName(typeof(Nullable<int>), true));
        testOutputHelper.WriteLine("5. " + GetFullName(typeof(List<int>), false));
        testOutputHelper.WriteLine("6. " + GetFullName(typeof(List<int>), true));
        testOutputHelper.WriteLine("7. " + GetFullName(typeof(List<Nullable<int>>), false));
        testOutputHelper.WriteLine("8. " + GetFullName(typeof(List<Nullable<int>>), true));
        testOutputHelper.WriteLine("9. " + GetFullName(typeof(ValueTuple<int, int, int, int, int, int>), true));

        var o = new
        {
            FirstName = "Tareq",
            LastName = "Imbasher",
            Age = 36
        };

        testOutputHelper.WriteLine("10. " + GetFullName(o.GetType(), false));
        testOutputHelper.WriteLine("11. " + GetFullName(o.GetType(), true));
    }

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

    private static string GetNameNewWay(Type type, bool fullyQualify = false)
    {
        if (type.GenericTypeArguments.Length == 0)
        {
            return fullyQualify ? type.FullName ?? type.Name : type.Name;
        }

        var builder = new ValueStringBuilder(128, 64);

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

        builder.Append(name.Split('`')[0]);
        builder.Append('<');

        for (var index = 0; index < type.GenericTypeArguments.Length; index++)
        {
            var gta = type.GenericTypeArguments[index];
            GetNameNewWayInner(ref builder, gta, fullyQualify);

            if (index < type.GenericTypeArguments.Length - 1)
            {
                builder.Append(", ");
            }
        }

        builder.Append('>');
    }
}