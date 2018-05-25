namespace Evo_20form
{
    partial class CycleSettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CycleSettingsForm));
            this.CalibrationPanel = new System.Windows.Forms.Panel();
            this.CheckPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // CalibrationPanel
            // 
            this.CalibrationPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CalibrationPanel.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.CalibrationPanel.Location = new System.Drawing.Point(2, 3);
            this.CalibrationPanel.Name = "CalibrationPanel";
            this.CalibrationPanel.Size = new System.Drawing.Size(275, 394);
            this.CalibrationPanel.TabIndex = 0;
            // 
            // CheckPanel
            // 
            this.CheckPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CheckPanel.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.CheckPanel.Location = new System.Drawing.Point(360, 3);
            this.CheckPanel.Name = "CheckPanel";
            this.CheckPanel.Size = new System.Drawing.Size(275, 394);
            this.CheckPanel.TabIndex = 1;
            this.CheckPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.CheckPanel_Paint);
            // 
            // CycleSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 470);
            this.Controls.Add(this.CheckPanel);
            this.Controls.Add(this.CalibrationPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CycleSettingsForm";
            this.Text = "CycleSettingsForm";
            this.Load += new System.EventHandler(this.CycleSettingsForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel CalibrationPanel;
        private System.Windows.Forms.Panel CheckPanel;
    }
}