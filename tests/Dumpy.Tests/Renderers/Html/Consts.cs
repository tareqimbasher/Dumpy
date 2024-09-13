using Dumpy.Tests.Renderers.Html.Models;

namespace Dumpy.Tests.Renderers.Html;

public static class Consts
{
    public const string DocumentTemplate =
        """
        <!doctype html>
        <html lang="en">
          <head>
            <title>Dumpy Kitchen Sink</title>
            <style>
              body {
                font-family: Arial, Helvetica, sans-serif;
              }
              table {
                  border-collapse: collapse;
              }
              th, td {
                padding: 4px 8px;
              }
              .dm-t-info th {
                background: #4f81bd;
                color: #fff;
                text-align: left;
              }
              .dm-t-data th {
                background: #233b55;
                color: #fff;
                text-align: left;
              }
              tr {
                  border-right: 1px solid #95b3d7;
                  border-left: 1px solid #95b3d7;
                }
              tr {
                  border-bottom: 1px solid #95b3d7;
                }
              tbody tr:nth-child(odd) {
                  background: #dbe5f0;
                }
              tbody th, tbody tr td {
                  border-right: 1px solid #95b3d7;
                }
              thead th:not(:last-of-type) {
                  border-right: 1px solid #95b3d7;
              }
              tr:hover {background-color: #D6EEEE !important;}
              .dm-null {
                color: gray;
                font-style: italic;
                font-size: 85%;
              }
            </style>
          </head>
          <body>
            HTML_REPLACE
          </body>
        </html>
        """;

    public static readonly Person Person = new Person
    {
        Name = "John Smith",
        Age = 30,
        OtherNames = ["Jack Smith", "Jim"],
        Spouse = new Person
        {
            Name = "Jane Rose",
            Age = 25
        },
        Children =
        [
            new Person
            {
                Name = "Adam",
                Age = 16
            },
            new Person
            {
                Name = "June",
                Age = 13
            }
        ]
    };

    public const string PersonHtmlRepresentation =
        """
        <table>
            <thead>
                <tr class="dm-t-info">
                    <th colspan="2">Person</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <th>Name</th>
                    <td>John Smith</td>
                </tr>
                <tr>
                    <th>Age</th>
                    <td>30</td>
                </tr>
                <tr>
                    <th>OtherNames</th>
                    <td>
                        <table>
                            <thead class="dm-t-info">
                                <tr>
                                    <th>String[] (2 items)</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>Jack Smith</td>
                                </tr>
                                <tr>
                                    <td>Jim</td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <th>Spouse</th>
                    <td>
                        <table>
                            <thead>
                                <tr class="dm-t-info">
                                    <th colspan="2">Person</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <th>Name</th>
                                    <td>Jane Rose</td>
                                </tr>
                                <tr>
                                    <th>Age</th>
                                    <td>25</td>
                                </tr>
                                <tr>
                                    <th>OtherNames</th>
                                    <td><span class="dm-null">null</span></td>
                                </tr>
                                <tr>
                                    <th>Spouse</th>
                                    <td><span class="dm-null">null</span></td>
                                </tr>
                                <tr>
                                    <th>Children</th>
                                    <td><span class="dm-null">null</span></td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <th>Children</th>
                    <td>
                        <table>
                            <thead>
                                <tr class="dm-t-info">
                                    <th colspan="5">Person[] (2 items)</th>
                                </tr>
                                <tr class="dm-t-data">
                                    <th>Name</th>
                                    <th>Age</th>
                                    <th>OtherNames</th>
                                    <th>Spouse</th>
                                    <th>Children</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>Adam</td>
                                    <td>16</td>
                                    <td><span class="dm-null">null</span></td>
                                    <td><span class="dm-null">null</span></td>
                                    <td><span class="dm-null">null</span></td>
                                </tr>
                                <tr>
                                    <td>June</td>
                                    <td>13</td>
                                    <td><span class="dm-null">null</span></td>
                                    <td><span class="dm-null">null</span></td>
                                    <td><span class="dm-null">null</span></td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>
        """;
}