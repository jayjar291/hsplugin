using HtmlAgilityPack;
using JayTek.OrionShiftManager.Core;
using JayTek.OrionShiftManager.FileServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace ConsoleApp1
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Roster roster = new Roster();
            var doc = new HtmlDocument();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var dialog = openFileDialog.ShowDialog();
            if (dialog == DialogResult.OK)
            {
                doc.Load(openFileDialog.FileName);
                var nodes = doc.DocumentNode.SelectNodes("//html/body/table[2]/tbody/tr/td/table/tbody/*");

                var rows = nodes.Select(tr => tr
                    .Elements("td")
                    .Select(td => td.InnerHtml.Trim().Replace("<br>", ","))
                    .ToArray());


                foreach (var row in rows.Skip(1))
                {
                    string[] formatttedRowregex = { Regex.Replace(row[0], @"(?i)<br[^>]*>(\r\n)+|\r+|\n+|\t+\s<[^>]+>|&nbsp;", "").Trim(),
                        Regex.Replace(row[2], @"(?i)<br[^>]*>(\r\n)+|\r+|\n+|\t+<[^>]+>|&nbsp;", "").Trim() };
                    string[] formatttedRow = { formatttedRowregex[0].Replace("\t", " "), formatttedRowregex[1].Replace("\t", " ") };
                    string[] formatttedRow2 = { formatttedRow[0].Replace("<span class=\"approved-timeoff\"></span>", ""), formatttedRow[1].Replace("<span class=\"approved-timeoff\"></span>", "") };
                    string[] formatttedRow3 = { formatttedRow2[0].Replace("<hr>", "^"), formatttedRow2[1].Replace("<hr>", "^") };
                    string[] formatttedRow4 = { formatttedRow3[0].Replace("<span class=\"released-shift\"><b>", ""), formatttedRow3[1].Replace("<span class=\"released-shift\"><b>", "") };
                    string[] formatttedRow5 = { formatttedRow4[0].Replace("</b></span>", " "), formatttedRow4[1].Replace("</b></span>", " ") };
                    string[] formatttedRow6 = { formatttedRow5[0].Replace("   -  ", ","), formatttedRow5[1].Replace("   -  ", ",") };
                    if (formatttedRow6[1].Contains("^"))
                    {
                        string[] shifts = formatttedRow6[1].Split('^');
                        List<List<string>> shiftdata = new List<List<string>>();
                        foreach (string item in shifts)
                        {
                            List<string> data = item.Split(',').ToList();
                            shiftdata.Add(data);
                        }
                        foreach (List<string> item in shiftdata)
                        {
                            if (!item[0].Contains(":"))
                            {
                                item.Add(item[0]);
                                item.RemoveAt(0);
                            }
                        }
                        DateTime t1 = DateTime.Parse(shiftdata[0][0]);
                        DateTime t2 = DateTime.Parse(shiftdata[0][1]);
                        Shift test;
                        if (shiftdata[0].Count > 3)
                        {
                            test = new Shift(t1.TimeOfDay, t2.TimeOfDay, shiftdata[0][2], shiftdata[0][3]);
                        }
                        else
                        {
                            test = new Shift(t1.TimeOfDay, t2.TimeOfDay, shiftdata[0][2]);
                        }
                        Employee temp = new Employee(formatttedRow6[0], test);
                        foreach (List<string> item in shiftdata.Skip(1))
                        {
                            DateTime time1 = DateTime.Parse(item[0]);
                            DateTime time2 = DateTime.Parse(item[1]);
                            Shift test2;
                            if (item.Count > 3)
                            {
                                test2 = new Shift(time1.TimeOfDay, time2.TimeOfDay, item[2], item[3]);
                            }
                            else
                            {
                                test2 = new Shift(time1.TimeOfDay, time2.TimeOfDay, item[2]);
                            }
                            temp.AddShift(test2);
                        }
                        roster.AddEmployee(temp);
                    }
                    else
                    {
                        string[] shift = formatttedRow6[1].Split(',');
                        DateTime t1 = DateTime.Parse(shift[0]);
                        DateTime t2 = DateTime.Parse(shift[1]);
                        Shift test = new Shift(t1.TimeOfDay, t2.TimeOfDay, shift[2]);
                        Employee temp = new Employee(formatttedRow6[0], test);
                        roster.AddEmployee(temp);
                    }
                }

                ISaveable saveable = new FileFunctions(openFileDialog.FileName.Replace(".html", ".smr"));
                saveable.ExportEmployee(roster);
            }
        }
    }
}
