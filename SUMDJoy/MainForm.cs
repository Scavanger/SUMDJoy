﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace SUMDJoy
{
    public partial class MainForm : Form
    {
        private SUMDToVJoy _sumdTovJoy;
        private Settings _settings;
        private string _currentSettingsFile;
        private List<ComboBox> _channelComboBoxes;
        private bool _ignorChannelComboBoxes = true;

        public MainForm()
        {
            _sumdTovJoy = new SUMDToVJoy();
            _sumdTovJoy.NewFrameRecieved += _sumdTovJoy_NewFrameRecieved;
            //_sumdTovJoy.Error += _sumdTovJoy_Error;

            _settings = new Settings();
            InitializeComponent();

            if (!_sumdTovJoy.DriverMatch)
                toolStripStatusLabel1.Text = "Driver Version, dosen't match!";

            _channelComboBoxes = new List<ComboBox>();
            for (int i = 1; i <= 16; i++)
            {
                _channelComboBoxes.Add(Controls.Find("comboBoxChannel" + i, true).FirstOrDefault() as ComboBox);
                _channelComboBoxes[i - 1].Tag = i;
                _channelComboBoxes[i - 1].BindingContext = new BindingContext();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = Application.ProductName;

            comboBoxComPorts.DataSource = _sumdTovJoy.SerialPorts;
            if (comboBoxComPorts.Items.Count == 0)
                toolStripStatusLabel1.Text = "No COM port found!";

            comboBoxvJoyDevice.DataSource = _sumdTovJoy.FreevJoyDevices;
            if (_sumdTovJoy.FreevJoyDevices.Length == 0)
                toolStripStatusLabel1.Text = "No free vJoy Device found!";

            if (_sumdTovJoy.IsvJoyEnabled)
                labelVJoyINfo.Text = _sumdTovJoy.VJoyInfo;
            else
                labelVJoyINfo.Text = "vJoy Driver not enabled.";

            SetChannelComboBoxes();
            _currentSettingsFile = Properties.Settings.Default.LastUserSettingsFile;
            labelSumdStatus.Text = "Not connected";
            _sumdTovJoy.Start();
            _ignorChannelComboBoxes = false;
            LoadSettings();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.LastUserSettingsFile = _currentSettingsFile;
            Properties.Settings.Default.LastComPort = comboBoxComPorts.SelectedItem as string;
            Properties.Settings.Default.Save();
        }

        #region EventHandlers

        private void ComboBoxComPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _sumdTovJoy.SetComPort(comboBoxComPorts.SelectedItem as string);
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }

            if (!_ignorChannelComboBoxes)
                _sumdTovJoy.Start();

            labelSumdStatus.Text = "Not Connected";
        }

        private void ComboBoxvJoyDevice_SelectedValueChanged(object sender, EventArgs e)
        {
            _sumdTovJoy.VJoyDevice = (uint)comboBoxvJoyDevice.SelectedItem;
            _sumdTovJoy.GetvJoyInfos();

            SetChannelComboBoxes();
            LoadSettings(true);

        }

        private void ComboBoxChannel_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!(sender is ComboBox) && !(sender as ComboBox).Name.Contains("comboBoxChannel"))
                return;

            if (!_ignorChannelComboBoxes)
            {
                foreach (var key in _sumdTovJoy.Assignments.Keys.ToList())
                    _sumdTovJoy.Assignments[key] = 0;

                _channelComboBoxes.ForEach(comboBox =>
                {
                    if (comboBox.SelectedItem != null)
                        _sumdTovJoy.Assignments[comboBox.SelectedItem as Assignment] = (int)comboBox.Tag;
                });
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                AddExtension = true,
                Multiselect = false,
                DefaultExt = "xml",
                ValidateNames = true,
                Filter = "SUMDJoy XML Config File (*.xml) |*.xml",
                RestoreDirectory = true
            };
            try
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _currentSettingsFile = ofd.FileName;
                    LoadSettings();
                    _sumdTovJoy.Start();
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "Can't load settings. " + ex.Message;
                _currentSettingsFile = string.Empty;
                Text = Application.ProductName;
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_currentSettingsFile))
                    SaveAs();
                else
                    SaveSettings();
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "Can't save settings. " + ex.Message;
                _currentSettingsFile = string.Empty;
                Text = Application.ProductName;
            }
        }

        private void SaveasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void _sumdTovJoy_NewFrameRecieved(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        private void _sumdTovJoy_Error(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        #endregion

        #region Methods

        private void UpdateStatus()
        {
            labelSumdStatus.Text = _sumdTovJoy.SUMDStatus ? "Valid" : "Invalid";
        }

        private void SetChannelComboBoxes()
        {
            bool setBack = _ignorChannelComboBoxes;
            _ignorChannelComboBoxes = true;

            _channelComboBoxes.ForEach(comboBox =>
            {
                comboBox.DataSource = new BindingSource(_sumdTovJoy.Assignments.Keys.ToList(), null);
                comboBox.DisplayMember = "Name";
                comboBox.ValueMember = "Name";
                comboBox.SelectedItem = _sumdTovJoy.Assignments.Where(a => a.Value == (int)comboBox.Tag).FirstOrDefault().Key;
                if (comboBox.SelectedItem == null)
                    comboBox.SelectedItem = new NoneAssingment();

            });
            _ignorChannelComboBoxes = setBack;
        }

        private void LoadSettings(bool ignoreDevice = false)
        {
            if (File.Exists(_currentSettingsFile))
            {
                _settings.Load(_currentSettingsFile);

                if (_sumdTovJoy.SerialPorts.Contains(_settings.VJoyConfig.COMPort))
                {
                    try
                    {
                        _sumdTovJoy.SetComPort(_settings.VJoyConfig.COMPort);
                        comboBoxComPorts.SelectedItem = _settings.VJoyConfig.COMPort;
                    }
                    catch (Exception ex)
                    {
                        toolStripStatusLabel1.Text = ex.Message;
                    }
                }

                if (!ignoreDevice && _sumdTovJoy.FreevJoyDevices.Contains(_settings.VJoyConfig.vJoyDevice))
                {
                    _sumdTovJoy.VJoyDevice = _settings.VJoyConfig.vJoyDevice;
                    comboBoxvJoyDevice.SelectedItem = _settings.VJoyConfig.vJoyDevice;
                }

                _channelComboBoxes.ForEach(comboBox =>
                {
                    int channel = (int)comboBox.Tag;
                    Assignment saved = _settings.VJoyConfig.Assignments.Where(a => a.Value == channel).FirstOrDefault().Key;
                    if (saved != null && comboBox.SelectedItem != null)
                    {
                        if (_sumdTovJoy.Assignments.ContainsKey(saved))
                        {
                            DeleteChannel(channel);
                            _sumdTovJoy.Assignments[saved] = (int)comboBox.Tag;
                            comboBox.SelectedItem = saved;
                        }
                    }
                });

                Text = string.Format("{0} - {1}", Application.ProductName, Path.GetFileName(_currentSettingsFile));
                toolStripStatusLabel1.Text = "Settings loaded.";
            }
            else
            {
                string lastComPort = Properties.Settings.Default.LastComPort;
                if (!string.IsNullOrWhiteSpace(lastComPort) && _sumdTovJoy.SerialPorts.Contains(lastComPort))
                {
                    comboBoxComPorts.SelectedItem = lastComPort;
                }
            }
        }

        private void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = "xlm",
                ValidateNames = true,
                Filter = "SUMDJoy XML Config File (*.xml) |*.xml",
                RestoreDirectory = true
            };
            try
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    _currentSettingsFile = sfd.FileName;
                    SaveSettings();
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "Can't save settings. " + ex.Message;
                _currentSettingsFile = string.Empty;
                Text = Application.ProductName;
            }
        }

        private void SaveSettings()
        {
            _settings.VJoyConfig.Assignments = _sumdTovJoy.Assignments;

            Text = string.Format("{0} - {1}", Application.ProductName, Path.GetFileName(_currentSettingsFile));

            _settings.VJoyConfig.COMPort = comboBoxComPorts.SelectedItem as string;
            _settings.VJoyConfig.vJoyDevice = (uint)comboBoxvJoyDevice.SelectedItem;
            _settings.Save(_currentSettingsFile);
            toolStripStatusLabel1.Text = "Settings saved.";
        }

        /// <summary>
        /// Makes shure no channel has more than one Assignment
        /// </summary>
        /// <param name="channel">Channel</param>
        private void DeleteChannel(int channel)
        {
            var assignments = _sumdTovJoy.Assignments.Where(a => a.Value == channel).ToDictionary(x => x.Key, x => x.Value);
            foreach (var key in assignments.Keys.ToList())
                _sumdTovJoy.Assignments[key] = 0;
        }

        #endregion


        private void ButtonRescan_Click(object sender, EventArgs e)
        {
            // (comboBoxComPorts.DataSource as BindingSource).ResetBindings(false);
        }
    }
}