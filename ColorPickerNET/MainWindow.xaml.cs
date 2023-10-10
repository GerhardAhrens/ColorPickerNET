namespace ColorPickerNET
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    using ColorPickerNET.Core;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap screenPixel = null;
        private DispatcherTimer dispatcherTimer = null;
        private GlobalKeyboardHook globalKey = new GlobalKeyboardHook();

        #region DLL Import
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref System.Drawing.Point lpPoint);

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int BitBlt(IntPtr hDC,
            int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
        #endregion DLL Import

        public MainWindow()
        {
            this.InitializeComponent();

            this.globalKey = new GlobalKeyboardHook();
            // Globaler Key festlegen
            globalKey.KeyDown += OnGlobalKeyKeyDown;
            globalKey.HookedKeys.Add(System.Windows.Forms.Keys.F2);
            globalKey.HookedKeys.Add(System.Windows.Forms.Keys.F3);

            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<Window, EventArgs>.AddHandler(this, "Closed", this.OnClosed);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.btnClose, "Click", this.OnBtnClose);

            try
            {
                this.Topmost = true;

                screenPixel = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(OnDispatcherTimerTick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                dispatcherTimer.Start();
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                throw;
            }
        }

        private void OnDispatcherTimerTick(object sender, EventArgs e)
        {
            try
            {
                System.Drawing.Point cursor = new System.Drawing.Point();

                GetCursorPos(ref cursor);

                Color c = GetColorByPosition(cursor);
                string htmlColor = ColorTranslator.ToHtml(c);
                
                if (htmlColor != this.txtCurrentHTMLColor.Text)
                {
                    this.txtCurrentHTMLColor.Text = htmlColor;
                    this.txtCurrentRGBColor.Text = $"{c.R.ToString()},{c.G.ToString()},{c.B.ToString()}";
                    this.txtCurrentAColor.Text = c.A.ToString();

                    int colorInt = ColorTranslator.ToWin32(c);
                    this.txtCurrentIntColor.Text = colorInt.ToString();

                    System.Windows.Media.Brush selectedBackColor = (System.Windows.Media.SolidColorBrush)(new System.Windows.Media.BrushConverter().ConvertFrom(htmlColor));
                    this.shapeColorDisplay.Fill = selectedBackColor;
                }
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                throw;
            }
        }

        private System.Drawing.Color GetColorByPosition(System.Drawing.Point pLocation)
        {
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, pLocation.X, pLocation.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return (screenPixel.GetPixel(0, 0));
        }

        private void OnBtnClose(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            this.lblDescription.Text = "HTML-Farbecode mit F2 in Zwischenablage";
            this.lblDescriptionWin32.Text = "Win32-Farbecode mit F3 in Zwischenablage";
        }

        private void OnClosed(object sender, EventArgs e)
        {
            this.dispatcherTimer.Stop();
            this.globalKey.unhook();
            this.screenPixel = null;
            this.dispatcherTimer = null;
        }

        private void OnGlobalKeyKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.F2)
            {
                string colorValue = this.txtCurrentHTMLColor.Text;
                System.Windows.Clipboard.SetText(colorValue);
                this.lblDescription.Text = string.Format("{0}*", this.lblDescription.Text);
                e.Handled = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F3)
            {
                string colorValue = this.txtCurrentIntColor.Text;
                System.Windows.Clipboard.SetText(colorValue);
                this.lblDescriptionWin32.Text = string.Format("{0}*", this.lblDescriptionWin32.Text);
                e.Handled = true;
            }
        }
    }
}
