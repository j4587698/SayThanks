using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NexusPHPAutoSayThanks
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void llLoginGetCookie_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (frmBrowser frmBrowser = new frmBrowser(txtURL.Text))
            {
                frmBrowser.SelectedCookieEvent += frmBrowser_SelectedCookieEvent;
                frmBrowser.ShowDialog();
            }
        }

        void frmBrowser_SelectedCookieEvent(string domain, string cookie)
        {
            txtCookie.Text = cookie;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (txtCookie.Text == "" || txtURL.Text == "")
            {
                MessageBox.Show("cookie或URL为空");
                return;
            }
            new Thread(() => { HtmlParse.GetAllItems(txtURL.Text, txtCookie.Text); }).Start();
            //btnStart.Enabled = false;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            HtmlParse.ShowLogEvent += HtmlParse_ShowLogEvent;
        }

        void HtmlParse_ShowLogEvent(string msg)
        {
            tsslInfo.Text = msg;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            HtmlParse.IsRunning = false;
        }
    }
}
