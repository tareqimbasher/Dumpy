using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
            return NullWidget.Instance;
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

            table.Title = options.TableOptions.ShowTitles
                ? new TableTitle(Markup.Escape(TypeUtil.GetName(targetType)), options.StyleOptions.TitleTextStyle)
                : null;
            table.Columns[0].Header($"{(elementsCountExceedMax ? "First " : "")}{rowCount} items");

            return table;
        }
        else
        {
            var members = options.GetReadableMembers(elementType);

            int maxCount = options.MaxCollectionItems;
            int rowCount = 0;
            bool elementsCountExceedMax = false;

            var rows = new List<List<IRenderable>>();

            foreach (var element in collection)
            {
                rowCount++;

                if (rowCount > maxCount)
                {
                    elementsCountExceedMax = true;
                    break;
                }

                var row = new List<IRenderable>();
                rows.Add(row);

                foreach (var member in members)
                {
                    var (memberType, memberValue) = member.GetMemberTypeAndValue(element);
                    row.Add(memberValue.DumpToRenderable(memberType, options));
                }
            }

            var typeName = Markup.Escape(TypeUtil.GetName(targetType, false));
            string title = $"{(elementsCountExceedMax ? "First " : "")}{rowCount} items | {typeName}";

            if (rowCount == 0)
            {
                return EmptyCollectionWidget.New(typeName);
            }

            var table = options.CreateTable();
            table.Title = options.TableOptions.ShowTitles
                ? new TableTitle(title, options.StyleOptions.TitleTextStyle)
                : null;

            foreach (var member in members)
            {
                table.AddColumn(new TableColumn(new Text(member.Name, options.StyleOptions.HeaderTextStyle)));
            }

            foreach (var row in rows)
            {
                table.AddRow(row);
            }

            return table;
        }
    }
}