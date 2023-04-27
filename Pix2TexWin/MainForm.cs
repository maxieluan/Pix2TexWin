namespace Pix2TexWin
{
    public partial class MainForm : Form
    {
        private NotifyIcon trayIcon;
        private bool capturing = false;
        private Rectangle capturedRegion;
        private PictureBox capturedPictureBox;

        public MainForm()
        {
            InitializeComponent();

            trayIcon = new NotifyIcon
            {
                Icon = new Icon("Resources\\icon.ico"),
                Text = "Pix2Tex Win",
                Visible = true,
            };
            

            trayIcon.DoubleClick += TrayIcon_DoubleClick;
            this.KeyDown += new KeyEventHandler(KeyDownHandler);
            
            capturedPictureBox  = new PictureBox();
            capturedPictureBox.Dock = DockStyle.Fill;
            capturedPictureBox.Name = "capturedPictureBox";
            capturedPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;

            this.Controls.Add(capturedPictureBox);

        }

        private void KeyDownHandler(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.Alt && e.KeyCode == Keys.D0)
            {
                // hide the form
                this.Hide();
                var overlayForm = new OverlayForm();
                overlayForm.EnableMouseEvent();

                overlayForm.OnImageCaptured += OnImageCapturedHandler;
                
                overlayForm.Show();
            }
        }

        private void OnImageCapturedHandler(object? sender, Bitmap bitmap)
        {
            this.Show();
            
            this.ClientSize = new Size(bitmap.Width, bitmap.Height);
            capturedPictureBox.Size = new Size(bitmap.Width, bitmap.Height);

            var image = (Image) bitmap.Clone();
            capturedPictureBox.Image = image;
        }

        private void TrayIcon_DoubleClick(object? sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            trayIcon.Visible = false;
        }
    }
}