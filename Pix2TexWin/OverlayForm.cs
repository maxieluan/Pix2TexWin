using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pix2TexWin
{
    public class OverlayForm : Form
    {
        private Point initialMousePosition;
        private Rectangle captureRegion;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel mousePositionLabel;
        private ToolStripStatusLabel captureRegionLabel;
        private bool capturing = false;

        public OverlayForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Gray;
            this.WindowState = FormWindowState.Maximized;
            this.Opacity = 0.5;

            statusStrip = new StatusStrip();
            mousePositionLabel = new ToolStripStatusLabel();
            captureRegionLabel = new ToolStripStatusLabel();
            statusStrip.Items.Add(mousePositionLabel);
            statusStrip.Items.Add(captureRegionLabel);
            //this.Controls.Add(statusStrip);

            this.Paint += PaintHandler;
        }

        public void EnableMouseEvent()
        {
            this.MouseDown += MouseDownHandler;
            this.MouseUp += MouseUpHandler;
            this.MouseMove += MouseMoveHandler;
        }

        private void MouseDownHandler(object? sender, MouseEventArgs e)
        {
            capturing = true;
            initialMousePosition = e.Location;
            captureRegion = new Rectangle(initialMousePosition, Size.Empty);
            this.Region = new Region(new Rectangle(Point.Empty, this.Size));
        }

        private void MouseMoveHandler(object? sender, MouseEventArgs e)
        {
            if (capturing)
            {
                if (e.Button == MouseButtons.Left)
                {
                    captureRegion.X = Math.Min(e.X, initialMousePosition.X);
                    captureRegion.Y = Math.Min(e.Y, initialMousePosition.Y);
                    captureRegion.Width = Math.Abs(e.X - initialMousePosition.X);
                    captureRegion.Height = Math.Abs(e.Y - initialMousePosition.Y);

                    this.Invalidate();

                    var excludeRegion = new Region(new Rectangle(Point.Empty, this.Size));
                    excludeRegion.Exclude(captureRegion);
                    this.Region = excludeRegion;
                }

                mousePositionLabel.Text = $"Mouse Position: ({e.X}, {e.Y})";
                captureRegionLabel.Text = $"Capture Region: {captureRegion.X}, {captureRegion.Y}, {captureRegion.Width}, {captureRegion.Height}";
            }
        }

        private void MouseUpHandler(object? sender, MouseEventArgs e)
        {
            capturing = false;
            this.MouseDown -= MouseDownHandler;
            this.MouseMove -= MouseMoveHandler;
            this.MouseUp -= MouseUpHandler;
            this.Hide();

            Point topLeft = new Point(captureRegion.X, captureRegion.Y);
            Size size = new Size(captureRegion.Width, captureRegion.Height);
            Rectangle screenRect = new Rectangle(topLeft, size);

            using(Bitmap bitmap = new Bitmap(screenRect.Width, screenRect.Height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(screenRect.Left, screenRect.Top, 0, 0, bitmap.Size);
                }

                OnImageCaptured?.Invoke(this, bitmap);
            }
        }

        public event EventHandler<Bitmap>? OnImageCaptured;

        private void PaintHandler(object? sender, PaintEventArgs e)
        {
            using(var brush = new SolidBrush (Color.FromArgb(128, Color.Gray)))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }

            var pen = new Pen(Color.Red, 2);
            e.Graphics.DrawRectangle(pen, captureRegion);
        }
    }
}
