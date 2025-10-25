namespace Server
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
            pictureBoxDisplay = new PictureBox();
            listBoxLogs = new ListBox();
            ((System.ComponentModel.ISupportInitialize)pictureBoxDisplay).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxDisplay
            // 
            pictureBoxDisplay.Location = new Point(12, 12);
            pictureBoxDisplay.Name = "pictureBoxDisplay";
            pictureBoxDisplay.Size = new Size(890, 484);
            pictureBoxDisplay.TabIndex = 0;
            pictureBoxDisplay.TabStop = false;
            // 
            // listBoxLogs
            // 
            listBoxLogs.FormattingEnabled = true;
            listBoxLogs.Location = new Point(12, 502);
            listBoxLogs.Name = "listBoxLogs";
            listBoxLogs.Size = new Size(890, 84);
            listBoxLogs.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(listBoxLogs);
            Controls.Add(pictureBoxDisplay);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBoxDisplay).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBoxDisplay;
        private ListBox listBoxLogs;
    }
}
