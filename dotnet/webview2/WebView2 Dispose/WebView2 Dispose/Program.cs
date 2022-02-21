using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace WebView2_Dispose
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (Form form = new Form())
            {
                form.Size = new System.Drawing.Size(640, 480);
                form.StartPosition = FormStartPosition.CenterScreen;

                using (WebView2 webView2 = new WebView2())
                {
                    webView2.Dock = DockStyle.Fill;
                    webView2.Source = new Uri("https://microsoft.com");

                    form.Controls.Add(webView2);

                    Application.Run(form);
                }
            }
        }
    }
}