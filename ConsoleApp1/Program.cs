using HtmlAgilityPack;
using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var doc = new HtmlDocument();
            doc.Load("C:/Users/jared/Desktop/new test/Report Window.html");

            var nodes = doc.DocumentNode.SelectNodes("//html/body/table[2]/tbody/tr/td/table/tbody/*");

            var rows = nodes.Select(tr => tr
                .Elements("td")
                .Select(td => td.InnerText.Trim())
                .ToArray());

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("Total day");
            dataTable.Columns.Add("Times");

            foreach (var row in rows.Skip(1))
            {
                string[] formatttedRow = { Regex.Replace(row[0], @"<[^>]+>|&nbsp;", " ").Trim(), row[1], Regex.Replace(row[2], @"<[^>]+>|&nbsp;", " ").Trim() };
            
                dataTable.Rows.Add(formatttedRow);
            }
            foreach (DataRow dataRow in dataTable.Rows)
            {
                foreach (var item in dataRow.ItemArray)
                {
                    Console.WriteLine(item);
                }
            }
        }
    }
}
