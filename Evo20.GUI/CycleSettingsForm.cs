using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Evo20.Controllers.Data;

namespace Evo20.GUI
{
    public partial class CycleSettingsForm : Form
    {
        readonly List<CheckBox> _calibrationCheckBoxes;

        CheckBox _lastChecked;

        public CycleSettingsForm()
        {
            InitializeComponent();
            _calibrationCheckBoxes = new List<CheckBox>();
        }


        private void CycleSettingsForm_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < CycleData.Instance.CalibrationTemperatures.Count; i++)
            {
                var box = new CheckBox
                {
                    Text = CycleData.Instance.CalibrationTemperatures[i].ToString(),
                    TabIndex = i,
                    Location = new Point(10, i * 20)
                };
                //box.Tag = i.ToString();
                //box.AutoSize = true;
                //vertical
                CalibrationPanel.Controls.Add(box);
                box.CheckedChanged += TemperatureCheckedChanged;
                _calibrationCheckBoxes.Add(box);
            }
        }

        private void TemperatureCheckedChanged(object sender, EventArgs e)
        {
            var box = sender as CheckBox;
            if (box == null)
                return;
            if (!box.Checked)
            {
                return;
            }
            if (_lastChecked == null)
            {
                _lastChecked = box;
                return;
            }
            int startIndex;
            int stopIndex;
            if (_lastChecked.TabIndex < box.TabIndex)
            {
                startIndex = _lastChecked.TabIndex;
                stopIndex = box.TabIndex;
            }
            else
            {
                startIndex = box.TabIndex;
                stopIndex = _lastChecked.TabIndex;
            }
            if (startIndex == stopIndex)
            {
                _lastChecked = box;
                return;
            }
            for (int i = startIndex; i < stopIndex; i++)
            {
                _calibrationCheckBoxes[i].Checked = true;

            }
            _lastChecked = box;
        }

        private void EndConfigurationButton_Click(object sender, EventArgs e)
        {
            var resultList = new List<int>();
            foreach (var item in _calibrationCheckBoxes)
            {
                if (item.Checked)
                {
                    resultList.Add(Convert.ToInt32(item.Text));
                }
            }
            CycleData.Instance.SetTemperatures(resultList);
            DialogResult = DialogResult.OK;
        }

    }
}
