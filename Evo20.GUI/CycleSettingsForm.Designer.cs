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
            this.label1 = new System.Windows.Forms.Label();
            this.EndConfigurationButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CalibrationPanel
            // 
            this.CalibrationPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CalibrationPanel.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.CalibrationPanel.Location = new System.Drawing.Point(98, 30);
            this.CalibrationPanel.Name = "CalibrationPanel";
            this.CalibrationPanel.Size = new System.Drawing.Size(275, 394);
            this.CalibrationPanel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(95, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(393, 13);
            this.label1.TabIndex = 1;
            this.label1.Tag = "";
            this.label1.Text = "Выбор температур. Если выбор не полон, потребуется загрузка из файла!!!";
            // 
            // EndConfigurationButton
            // 
            this.EndConfigurationButton.Location = new System.Drawing.Point(98, 435);
            this.EndConfigurationButton.Name = "EndConfigurationButton";
            this.EndConfigurationButton.Size = new System.Drawing.Size(162, 23);
            this.EndConfigurationButton.TabIndex = 2;
            this.EndConfigurationButton.Text = "Завершить настройку";
            this.EndConfigurationButton.UseVisualStyleBackColor = true;
            this.EndConfigurationButton.Click += new System.EventHandler(this.EndConfigurationButton_Click);
            // 
            // CycleSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 470);
            this.Controls.Add(this.EndConfigurationButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CalibrationPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CycleSettingsForm";
            this.Text = "CycleSettingsForm";
            this.Load += new System.EventHandler(this.CycleSettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel CalibrationPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button EndConfigurationButton;
    }
}