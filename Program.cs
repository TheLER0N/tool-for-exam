using System;
using System.Windows.Forms;

namespace Ekzamen
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                var msg = ex != null ? ex.ToString() : (e.ExceptionObject ?? "unknown").ToString();
                Console.Error.WriteLine("FATAL: " + msg);
                try { MessageBox.Show(msg, "FATAL EXCEPTION", MessageBoxButtons.OK, MessageBoxIcon.Error); } catch { }
            };
            Application.ThreadException += (s, e) =>
            {
                Console.Error.WriteLine("UI: " + e.Exception);
                try { MessageBox.Show(e.Exception.ToString(), "UI EXCEPTION", MessageBoxButtons.OK, MessageBoxIcon.Error); } catch { }
            };
            try
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new DemoForm());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("MAIN: " + ex);
                try { MessageBox.Show(ex.ToString(), "MAIN EXCEPTION", MessageBoxButtons.OK, MessageBoxIcon.Error); } catch { }
            }
        }
    }
}