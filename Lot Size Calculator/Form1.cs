using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Lot_Size_Calculator
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public Form1()
        {
            InitializeComponent();

            // Form köþelerini yuvarlat, borderless yap
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            SetRoundedRegion(32);

            // WebView2 Drag (arka plana týklayýnca sürükle)
            webView21.CoreWebView2InitializationCompleted += WebView2_CoreWebView2InitializationCompleted;
        }

        // KÖÞELERÝ YUVARLATAN METOT
        private void SetRoundedRegion(int radius)
        {
            var bounds = new Rectangle(0, 0, this.Width, this.Height);
            var path = new GraphicsPath();
            int d = radius * 2;
            path.StartFigure();
            path.AddArc(bounds.Left, bounds.Top, d, d, 180, 90);
            path.AddLine(bounds.Left + radius, bounds.Top, bounds.Right - radius, bounds.Top);
            path.AddArc(bounds.Right - d, bounds.Top, d, d, 270, 90);
            path.AddLine(bounds.Right, bounds.Top + radius, bounds.Right, bounds.Bottom - radius);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddLine(bounds.Right - radius, bounds.Bottom, bounds.Left + radius, bounds.Bottom);
            path.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90, 90);
            path.AddLine(bounds.Left, bounds.Bottom - radius, bounds.Left, bounds.Top + radius);
            path.CloseFigure();
            this.Region = new Region(path);
        }

        // Form boyutlanýnca köþe yuvarla
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetRoundedRegion(32); // Yuvarlaklýk px (deðiþtirilebilir)
        }

        // WebView2 drag-window desteði (arka plana týklanýnca)
        private void WebView2_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            webView21.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
        }

        private void CoreWebView2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            var msg = e.TryGetWebMessageAsString();
            if (msg == "dragwindow")
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        // HTML yüklemesi
        private void Form1_Load(object sender, EventArgs e)
        {
            string htmlPath = System.IO.Path.Combine(Application.StartupPath, "background.html");
            webView21.Source = new Uri(htmlPath);
        }

        private void webView21_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
