﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Evo20.Controllers;

namespace Evo20.GUI
{
    public partial class CycleSettingsForm : Form
    {
        List<CheckBox> calibrationCheckBoxes;

        CheckBox lastChecked;

        public CycleSettingsForm()
        {
            InitializeComponent();
            calibrationCheckBoxes = new List<CheckBox>();
        }


        private void CycleSettingsForm_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < CycleData.Current.CalibrationTemperatures.Count; i++)
            {
                var box = new CheckBox();
                //box.Tag = i.ToString();
                box.Text = CycleData.Current.CalibrationTemperatures[i].ToString();
                box.TabIndex = i;
                //box.AutoSize = true;
                box.Location = new Point(10, i * 20); //vertical
                CalibrationPanel.Controls.Add(box);
                box.CheckedChanged += temperatureCheckedChanged;
                calibrationCheckBoxes.Add(box);
            }
        }

        private void temperatureCheckedChanged(object sender, EventArgs e)
        {
            var box = sender as CheckBox;
            if (box == null)
                return;
            if (!box.Checked)
            {
                return;
            }
            if (lastChecked == null)
            {
                lastChecked = box;
                return;
            }
            int startIndex = 0;
            int stopIndex = 0;
            if (lastChecked.TabIndex < box.TabIndex)
            {
                startIndex = lastChecked.TabIndex;
                stopIndex = box.TabIndex;
            }
            else
            {
                startIndex = box.TabIndex;
                stopIndex = lastChecked.TabIndex;
            }
            if (startIndex == stopIndex)
            {
                lastChecked = box;
                return;
            }
            for (int i = startIndex; i < stopIndex; i++)
            {
                calibrationCheckBoxes[i].Checked = true;

            }
            lastChecked = box;
        }

        private void CheckPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void EndConfigurationButton_Click(object sender, EventArgs e)
        {
            var resultList = new List<int>();
            foreach (var item in calibrationCheckBoxes)
            {
                if (item.Checked)
                {
                    resultList.Add(Convert.ToInt32(item.Text));
                }
            }
            CycleData.Current.SetTemperatures(resultList);
            this.DialogResult = DialogResult.OK;
        }

    }
}
