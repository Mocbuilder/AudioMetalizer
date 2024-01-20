using System;
using System.Windows.Forms;

namespace NotifyIcon_Test
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create an instance of your NotifyIcon-based class
            var notifyIconForm = new NotifyIconForm();

            // Run the application without a main form
            Application.Run(notifyIconForm);
        }
    }
}
