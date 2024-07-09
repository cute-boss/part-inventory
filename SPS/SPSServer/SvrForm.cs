/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* SvrForm.cs                                                                                           */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      09/03/2023  Add thPartQtyStatus func                                        Azmir               */
/*      07/08/2023  Add thSQLDBBackup func                                          Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPSLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace SPSServer
{
    public partial class SvrForm : Form
    {
        const int M_IMAXSTATUS = 5;
        readonly Label[] lblStatus = new Label[M_IMAXSTATUS];
        readonly List<string> listStatus = new List<string>();
        NotifyIcon notifyIcon;
        ContextMenu cMenu;

        bool m_bShow = true;
        bool m_bLicense;

        public SvrForm()
        {
            InitializeComponent();
            AddControl();
        }

        private void SvrForm_Load(object sender, EventArgs e)
        {
            Common.SetDBStrCon(ConfigurationManager.ConnectionStrings["SPSDBConStr"].ConnectionString);

            Lbl_ipStatus.Text = Common.GetIPAddress(); ;
            //lbl_portStatus.Text = m_iSvrPort.ToString();
            Lbl_licenseStatus.Text = (m_bLicense) ? "Licensed" : "Demo (" + Common.g_dtDemo.ToString("dd-MM-yyyy") + ")";
            Lbl_versionStatus.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Lbl_copyright.Text = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(),
                                    typeof(AssemblyCopyrightAttribute), false)).Copyright + " " +
                                ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(),
                                    typeof(AssemblyCompanyAttribute), false)).Company;

            Thread thInit = new Thread(new ThreadStart(InitCheck))
            {
                IsBackground = true
            };

            thInit.Start();
        }

        public void AddControl()
        {
            for (int i = 0; i < M_IMAXSTATUS; i++)
            {
                lblStatus[i] = new Label
                {
                    Name = "lbl_Status+" + i.ToString(),
                    Location = new Point(5, 5 + (i * 30)),
                    Width = 200
                };

                Panel_result.Controls.Add(lblStatus[i]);
            }

            cMenu = new ContextMenu();
            cMenu.MenuItems.Add(0,
                new MenuItem("Hide", new EventHandler(ShowHide_Click)));
            cMenu.MenuItems.Add(1,
                new MenuItem("Exit", new EventHandler(Exit_Click)));

            notifyIcon = new NotifyIcon
            {
                Text = "SPS Server",
                Visible = true,
                Icon = new Icon("sps_icon.ico"),
                ContextMenu = cMenu
            };
        }

        /// <summary>
        /// Exit function
        /// </summary>
        private void Exit()
        {
            if (CommonMsg.ExitConfirmationMsg() == DialogResult.Yes)
            {
                DALLog.SetLog("SPS Server has stopped", (int)EnumEx.LogType.EVTLOG_SVR, "Event");

                this.Close();
                Environment.Exit(0);
            }
        }

        private void ShowHide()
        {
            if (m_bShow)
            {
                this.Hide();
                cMenu.MenuItems[0].Text = "Show";
            }
            else
            {
                this.Show();
                cMenu.MenuItems[0].Text = "Hide";
            }

            m_bShow = !m_bShow;
        }

        /// <summary>
        /// Server threads
        /// </summary>
        private void InitCheck()
        {
            int iErr = 0;
            bool bStartThread = false;
            BLLEvent bllEvt = new BLLEvent();
            string sDBStr = Common.GetDBStrCon();

            while (true)
            {
                if (!Common.ChkMSSQLDBStatus(sDBStr))
                {
                    Display("Database connection: fail...");
                    iErr++;
                }
                else
                {
                    Display("Database connection: OK...");
                    iErr = 0;

                    if (!bStartThread)
                    {
                        #region check license
                        // Use Mac address
                        m_bLicense = Common.ChkLicense();

                        if (!m_bLicense)
                        {
                            Thread thDemo = new Thread(new ThreadStart(bllEvt.ChkDemoEndTh))
                            {
                                IsBackground = true
                            };

                            thDemo.Start();
                        }

                        Lbl_licenseStatus.Invoke((MethodInvoker)(() => Lbl_licenseStatus.Text = (m_bLicense) ? "Licensed" : "Demo (" + Common.g_dtDemo.ToString("dd-MM-yyyy") + ")"));

                        DALLog.SetLog("SPS Server has started", (int)EnumEx.LogType.EVTLOG_SVR, "Event");

                        #endregion

                        if (!DALMisc.GetMisc(out Misc misc))
                        {
                            CommonMsg.SQLErrorMsg();
                            Environment.Exit(1);
                        }

                        Thread thRemoveHistory = new Thread(new ThreadStart(bllEvt.RemoveHistoryTh))
                        {
                            IsBackground = true
                        };

                        thRemoveHistory.Start();

                        Thread thPartQtyStatus = new Thread(new ThreadStart(bllEvt.PartQtyStatusTh))
                        {
                            IsBackground = true
                        };

                        thPartQtyStatus.Start();

                        Thread thSQLDBBackup = new Thread(new ThreadStart(bllEvt.SQLDBBackupTh))
                        {
                            IsBackground = true
                        };

                        thSQLDBBackup.Start();

                        bStartThread = true;
                    }
                }

                if (iErr == 3)
                {
                    DALLog.SetLog("SPS Server has stopped forcibly", (int)EnumEx.LogType.EVTLOG_SVR, "Event");
                    Environment.Exit(1);
                }

                if (iErr == 0)
                {
                    Thread.Sleep(10000);
                }
                else
                {
                    Thread.Sleep(3000);
                }
            }
        }

        /// <summary>
        /// Display of database connection status
        /// </summary>
        /// <param name="sText"></param>
        private void Display(string sText)
        {
            if (listStatus.Count == M_IMAXSTATUS)
            {
                listStatus[0] = listStatus[1];
                listStatus[1] = listStatus[2];
                listStatus[2] = listStatus[3];
                listStatus[3] = listStatus[4];
                listStatus[4] = sText;
            }
            else
            {
                listStatus.Add(sText);
            }

            for (int i = 0; i < listStatus.Count; i++)
            {
                if (lblStatus[i].IsHandleCreated)
                {
                    lblStatus[i].Invoke((MethodInvoker)(() => lblStatus[i].Text = (string)listStatus[i]));
                }
            }
        }

        private void ShowHide_Click(Object sender, System.EventArgs e)
        {
            ShowHide();
        }

        private void Exit_Click(Object sender, System.EventArgs e)
        {
            Exit();
        }

        private void Btn_exit_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void SvrForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_bShow)
            {
                this.Hide();
                cMenu.MenuItems[0].Text = "Show";
                m_bShow = !m_bShow;
            }
            e.Cancel = true;
        }
    }
}
