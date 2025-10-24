using System.Windows.Forms;

namespace ScreenSharePracticeAndHw
{
    public partial class Form1 : Form
    {
        private ControlsClient client;
        private readonly SynchronizationContext? ctx;
        public Form1()
        {
            InitializeComponent();
            ctx = SynchronizationContext.Current;
            client = new ControlsClient(ctx);
        }

        private void LogMessage(string mess)
        {

            listBoxLogs.Items.Add(mess);
            listBoxLogs.TopIndex = listBoxLogs.Items.Count - 1;

        }
        private void DisplayScreenshot(Bitmap screenshot)
        {

            if (screen_view_picture_box.Image != null)
            {
                screen_view_picture_box.Image.Dispose();
            }
            screen_view_picture_box.Image = screenshot;
            screen_view_picture_box.SizeMode = PictureBoxSizeMode.Zoom;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            client.LogServer += LogMessage;
            client.ScreenshotCaptured += DisplayScreenshot;
        }

        private void connect_btn_Click(object sender, EventArgs e)
        {
            Task.Run(() => client.Connect());
            connect_btn.Enabled = false;
        }


        private void stop_btn_Click(object sender, EventArgs e)
        {
            client.StopSharing();
            send_btn.Enabled = true;
            stop_btn.Enabled = false;

        }

        private void send_btn_Click(object sender, EventArgs e)
        {
            client.StartSharing();
            send_btn.Enabled = false;
            stop_btn.Enabled = true;


        }
    }
}
