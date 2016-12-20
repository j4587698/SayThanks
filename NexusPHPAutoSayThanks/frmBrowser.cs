using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NexusPHPAutoSayThanks
{
    public partial class frmBrowser : Form
    {

        public delegate void SelectedCookie(string domain, string cookie);

        public event SelectedCookie SelectedCookieEvent;
        public frmBrowser(string url)
        {
            InitializeComponent();
            if (url != String.Empty)
            {
                txtUrl.Text = url;
                btnGo.PerformClick();
            }
            
        }

        private void txtUrl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btnGo.PerformClick();
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            wbLogin.Navigate(txtUrl.Text);
        }

        private void wbLogin_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //MessageBox.Show(wbLogin.Document.Cookie + ":" + wbLogin.Url.ToString());
            //ListViewItem lvi = new ListViewItem(new string[] { wbLogin.Url.ToString(), wbLogin.Document.Cookie });
            //lvCookie.Items.Add(lvi);
            if (lvCookie.Items.IndexOfKey(wbLogin.Url.ToString()) < 0)
            {
                ListViewItem lvi = new ListViewItem(new string[] { wbLogin.Url.ToString(), GetCookie.GetCookies(wbLogin.Url.ToString()) });
                lvi.Name = wbLogin.Url.ToString();
                lvCookie.Items.Add(lvi);
            }

        }

        private void lvCookie_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvCookie.SelectedItems.Count > 0)
            {
                txtCookie.Text = lvCookie.SelectedItems[0].SubItems[1].Text;
            }
            
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lvCookie.Items.Clear();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string domain = new Uri(lvCookie.SelectedItems[0].SubItems[0].Text).Host;
            string cookie = lvCookie.SelectedItems[0].SubItems[1].Text;
            if (SelectedCookieEvent != null)
            {
                SelectedCookieEvent(domain, cookie);
            }
            Close();
        }
    }
}
