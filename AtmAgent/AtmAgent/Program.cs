using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;
using AtmAgentChequeUpload.Controller;

namespace AtmAgent
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        //[STAThread]
        //static void Main()
        //{
        //    Application.SetHighDpiMode(HighDpiMode.SystemAware);
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new Form1());
        //}

        public static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            }

            if (args.Length > 0 && args[0] == "/debug")
            {
                Application.Run(new DebugForm<ChequeUploadController>());
            }
            else
            {
                //var servicesToRun = new ServiceBase[] { new ServiceHost<ChequeUploadController>() };
                //ServiceBase.Run(servicesToRun);
            }
        }


    }
}
