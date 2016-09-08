using System;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

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
            var shadowCopyFiles = false;
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
                            var name = line.Remove(eq).Trim().ToUpper();
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
                                case "SHADOWCOPYFILES":
                                    shadowCopyFiles = value.Equals("Y", StringComparison.InvariantCultureIgnoreCase);
                                    break;
                                default:

                                    throw new Exception("Unknown setting " + name + "=" + value);
                            }
                        }
                    }
                }

                var p = new System.Diagnostics.Process();
                string commandLine = CommandLineArgs;
                if (string.IsNullOrWhiteSpace(commandLine))
                    commandLine = SplitCommandLine(System.Environment.CommandLine)[1];

                if (shadowCopyFiles)
                {
                    if (!string.IsNullOrEmpty(workingDir))
                        Directory.SetCurrentDirectory(workingDir);
                    var setup = AppDomain.CurrentDomain.SetupInformation;
                    setup.ShadowCopyFiles = "true";
                    var appDomain = AppDomain.CreateDomain(exeFile, AppDomain.CurrentDomain.Evidence, setup);
                    appDomain.ExecuteAssembly(exeFile, CommandLineToArgs(commandLine));
                }
                else
                {
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
                    sw.WriteLine("Shadow Copy Files=" + (shadowCopyFiles ? "Y" : "N"));
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

        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        public static string[] CommandLineToArgs(string commandLine)
        {
            int argc;
            var argv = CommandLineToArgvW(commandLine, out argc);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();
            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }
    }
}
