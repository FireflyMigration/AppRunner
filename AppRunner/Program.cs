using System;
using System.Windows.Forms;
using System.IO;
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
            string exeFile = "";
            string workingDir = "";
            string CommandLineArgs = "";
            try
            {
                using (var sr = new StreamReader(Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName) + ".settings"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var eq = line.IndexOf('=');
                        if (eq > 0)
                        {
                            var name = line.Remove(eq).Trim().ToUpper() ;
                            var value = line.Substring(eq + 1).Trim();
                            if (name.StartsWith(";"))
                                continue;
                            switch (name)
                            {
                                case "EXEFILE":
                                    exeFile = value;
                                    break;
                                case "WORKINGDIR":
                                    workingDir = value;
                                    break;
                                case "COMMANDLINEARGS":
                                    CommandLineArgs = value;
                                    break;
                                default:
                                    
                                    throw new Exception("Unknown setting " + name + "=" + value);
                            }
                        }
                    }

                    var p = new System.Diagnostics.Process();
                    string commandLine = CommandLineArgs;
                    if (string.IsNullOrWhiteSpace(commandLine))
                        commandLine = SplitCommandLine(System.Environment.CommandLine)[1];

                    p.StartInfo.Arguments = commandLine;
                    p.StartInfo.FileName = exeFile;
                    p.StartInfo.WorkingDirectory = workingDir;
                    p.Start();
                }
            }
            catch (Exception ex)
            {
                using (var sw = new StringWriter())
                {
                    sw.WriteLine("Failed: " + ex.Message);
                    sw.WriteLine("ExeFile=" + exeFile);
                    sw.WriteLine("WorkingDirectory=" + workingDir);
                    sw.WriteLine("CommandLineArgs=" + CommandLineArgs);
                    sw.WriteLine("Called with=" + Environment.CommandLine);
                    MessageBox.Show(sw.ToString(), "AppRunner Failed To Start", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
