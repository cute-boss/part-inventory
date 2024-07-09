/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* Program.cs                                                                                           */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPSLib;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace SPSServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Common.g_bServer = true;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Restrict of opening duplicate instance
            string sProcessName = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcesses().Count(p => p.ProcessName == sProcessName) > 1)
            {
                CommonMsg.DupServerMsg();
                return;
            }

            Application.Run(new SvrForm());
        }
    }
}
