namespace ScreenSharePracticeAndHw
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            show_task_box = new PictureBox();
            screen_view_picture_box = new PictureBox();
            connect_btn = new Button();
            send_btn = new Button();
            listBoxLogs = new ListBox();
            stop_btn = new Button();
            ((System.ComponentModel.ISupportInitialize)show_task_box).BeginInit();
            ((System.ComponentModel.ISupportInitialize)screen_view_picture_box).BeginInit();
            SuspendLayout();
            // 
            // show_task_box
            // 
            show_task_box.BackgroundImage = Properties.Resources.зображення;
            show_task_box.BackgroundImageLayout = ImageLayout.Stretch;
            show_task_box.Location = new Point(12, 12);
            show_task_box.Name = "show_task_box";
            show_task_box.Size = new Size(452, 112);
            show_task_box.TabIndex = 0;
            show_task_box.TabStop = false;
            // 
            // screen_view_picture_box
            // 
            screen_view_picture_box.Location = new Point(12, 130);
            screen_view_picture_box.Name = "screen_view_picture_box";
            screen_view_picture_box.Size = new Size(776, 308);
            screen_view_picture_box.TabIndex = 1;
            screen_view_picture_box.TabStop = false;
            // 
            // connect_btn
            // 
            connect_btn.Location = new Point(470, 12);
            connect_btn.Name = "connect_btn";
            connect_btn.Size = new Size(104, 23);
            connect_btn.TabIndex = 2;
            connect_btn.Text = "Connect";
            connect_btn.UseVisualStyleBackColor = true;
            connect_btn.Click += connect_btn_Click;
            // 
            // send_btn
            // 
            send_btn.Location = new Point(690, 12);
            send_btn.Name = "send_btn";
            send_btn.Size = new Size(98, 23);
            send_btn.TabIndex = 3;
            send_btn.Text = "Send";
            send_btn.UseVisualStyleBackColor = true;
            send_btn.Click += send_btn_Click;
            // 
            // listBoxLogs
            // 
            listBoxLogs.FormattingEnabled = true;
            listBoxLogs.ItemHeight = 15;
            listBoxLogs.Location = new Point(470, 41);
            listBoxLogs.Name = "listBoxLogs";
            listBoxLogs.Size = new Size(318, 79);
            listBoxLogs.TabIndex = 4;
            // 
            // stop_btn
            // 
            stop_btn.Location = new Point(580, 12);
            stop_btn.Name = "stop_btn";
            stop_btn.Size = new Size(104, 23);
            stop_btn.TabIndex = 5;
            stop_btn.Text = "Stop";
            stop_btn.UseVisualStyleBackColor = true;
            stop_btn.Click += stop_btn_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(stop_btn);
            Controls.Add(listBoxLogs);
            Controls.Add(send_btn);
            Controls.Add(connect_btn);
            Controls.Add(screen_view_picture_box);
            Controls.Add(show_task_box);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)show_task_box).EndInit();
            ((System.ComponentModel.ISupportInitialize)screen_view_picture_box).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox show_task_box;
        private PictureBox screen_view_picture_box;
        private Button connect_btn;
        private Button send_btn;
        private ListBox listBoxLogs;
        private Button stop_btn;
    }
}
