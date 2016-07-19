using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AppRunner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                var p = new System.Diagnostics.Process();
                string commandLine = Properties.Settings.Default.CommandLineArgs;
                if (string.IsNullOrWhiteSpace(commandLine))
                    commandLine = SplitCommandLine(System.Environment.CommandLine)[1];

                p.StartInfo.Arguments = commandLine;
                p.StartInfo.FileName = Properties.Settings.Default.ExeFile;
                p.StartInfo.WorkingDirectory = Properties.Settings.Default.WorkingDirectory;
                p.Start();
            }
            catch (Exception ex)
            {
                using (var sw = new System.IO.StringWriter())
                {
                    sw.WriteLine("Failed: " + ex.Message);
                    sw.WriteLine("ExeFile=" + Properties.Settings.Default.ExeFile);
                    sw.WriteLine("WorkingDirectory=" + Properties.Settings.Default.WorkingDirectory);
                    sw.WriteLine("CommandLineArgs=" + Properties.Settings.Default.CommandLineArgs);
                    sw.WriteLine("Called with=" + Environment.CommandLine);
                    MessageBox.Show( sw.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        internal static string[] SplitCommandLine(string commandLine)
        {
            var result = new[] { "", "" };
            if (commandLine[0] == '\"')
            {
                var i = commandLine.IndexOf('\"', 1);
                result[0] = commandLine.Substring(1, i - 1);
                result[1] = commandLine.Substring(i + 1).Trim();
            }
            else
            {
                result[0] = commandLine.Split(' ')[0];
                if (commandLine.Length > result[0].Length)
                    result[1] = commandLine.Substring(result[0].Length).Trim();
            }

            return result;
        }
    }
}
