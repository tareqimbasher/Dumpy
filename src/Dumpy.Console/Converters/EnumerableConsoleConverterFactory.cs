using System;
using System.Collections;
using Dumpy.Console.Widgets;
using Dumpy.Utils;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

// ReSharper disable once InconsistentNaming
public class EnumerableConsoleConverterFactory : ConsoleConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return TypeUtil.IsCollection(typeToConvert);
    }

    public override ConsoleConverter? CreateConverter(Type typeToConvert, ConsoleDumpOptions options)
    {
        var converterType = typeof(EnumerableDefaultConsoleConverter<>).MakeGenericType(typeToConvert);
        return Activator.CreateInstance(converterType) as ConsoleConverter;
    }
}

// ReSharper disable once InconsistentNaming
public class EnumerableDefaultConsoleConverter<T> : ConsoleConverter<T>
{
    public override IRenderable Convert(T? value, Type targetType, ConsoleDumpOptions options)
    {
        if (value is null)
        {
            return NullWidget.New(options);
        }

        var collection = value as IEnumerable ??
                         throw new Exception($"Value of type {targetType} is not an {nameof(IEnumerable)}.");

        var elementType = TypeUtil.GetCollectionElementType(targetType) ?? typeof(object);

        bool isElementObject = TypeUtil.IsObject(elementType);

        if (!isElementObject)
        {
            var table = options.CreateTable();
            table.AddColumn("");

            int maxCount = options.MaxCollectionItems;
            int rowCount = 0;
            bool elementsCountExceedMax = false;

            foreach (var element in collection)
            {
                rowCount++;

                if (rowCount > maxCount)
                {
                    elementsCountExceedMax = true;
                    break;
                }

                table.AddRow(element.DumpToRenderable(elementType, options));
            }

            table.Title = options.Tables.ShowTitles
                ? new TableTitle(Markup.Escape(TypeUtil.GetName(targetType)), options.Styles.TableTitleText)
                : null;
            table.Columns[0].Header($"{(elementsCountExceedMax ? "First " : "")}{rowCount} items");

            return table;
        }
        else
        {
            var members = options.GetReadableMembers(elementType);
            var typeName = Markup.Escape(TypeUtil.GetName(targetType, false));

            int maxCount = options.MaxCollectionItems;
            int rowCount = 0;
            bool elementsCountExceedMax = false;
            Table? table = null;

            foreach (var element in collection)
            {
                rowCount++;

                if (rowCount > maxCount)
                {
                    elementsCountExceedMax = true;
                    break;
                }

                if (table == null)
                {
                    table = options.CreateTable();
                    foreach (var member in members)
                    {
                        table.AddColumn(new TableColumn(new Text(member.Name, options.Styles.TableHeaderText)));
                    }
                }

                var row = new IRenderable[members.Length];
                for (int i = 0; i < members.Length; i++)
                {
                    var (memberType, memberValue) = members[i].GetMemberTypeAndValue(element);
                    row[i] = memberValue.DumpToRenderable(memberType, options);
                }

                table.AddRow(row);
            }

            if (table == null)
            {
                return EmptyCollectionWidget.New(typeName, options);
            }

            string title = $"{(elementsCountExceedMax ? "First " : "")}{rowCount} items | {typeName}";
            table.Title = options.Tables.ShowTitles
                ? new TableTitle(title, options.Styles.TableTitleText)
                : null;

            return table;
        }
    }
}