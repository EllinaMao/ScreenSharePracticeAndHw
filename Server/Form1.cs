namespace Server
{
    public partial class Form1 : Form
    {
        private ServerControler receiver;
        private SynchronizationContext? ctx;

        public Form1()
        {
            InitializeComponent();

            ctx = SynchronizationContext.Current;

            receiver = new ServerControler(ctx);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            receiver.LogEvent += LogMessage;
            receiver.FrameReady += DisplayScreenshot;
            receiver.StartListening();
        }

        private void LogMessage(string message)
        {
            listBoxLogs.Items.Add(message);
            listBoxLogs.TopIndex = listBoxLogs.Items.Count - 1;
        }

        private void DisplayScreenshot(Bitmap frame)
        {
            Image oldImage = pictureBoxDisplay.Image;
            pictureBoxDisplay.Image = frame;
            oldImage?.Dispose();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            receiver?.Dispose();
        }
    }
}