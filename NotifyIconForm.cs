using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using NAudio.CoreAudioApi;

namespace NotifyIcon_Test
{
    public class NotifyIconForm : ApplicationContext
    {
        bool isMetal = false;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenu;

        public NotifyIconForm()
        {
            InitializeNotifyIcon();
            SetAutostart(true);
        }

        private void SetAutostart(bool enable)
        {
            try
            {
                string appName = "MyApp"; // Choose a unique name for your application
                string executablePath = Application.ExecutablePath;

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (enable)
                    {
                        // Check if the entry already exists before creating it
                        if (key.GetValue(appName) == null)
                        {
                            key.SetValue(appName, "\"" + executablePath + "\"");
                        }
                    }
                    else
                    {
                        // Remove the entry if it exists
                        key.DeleteValue(appName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting autostart: {ex.Message}");
            }
        }


        public void GetVolume()
        {
            try
            {
                var endpointVolume = GetDefaultAudioEndpointVolume();

                float currentVolumeLevel = endpointVolume.MasterVolumeLevelScalar;

                if (currentVolumeLevel >= 0.7f)
                {
                    notifyIcon.Icon = Icon.FromHandle(Properties.Resources.high_volume.GetHicon());
                    currentVolumeLevel = 1f;
                    isMetal = false;
                }
                else if (currentVolumeLevel <= 0.6f)
                {
                    notifyIcon.Icon = Icon.FromHandle(Properties.Resources.Sabaton_S_Logo.GetHicon());
                    currentVolumeLevel = 0.5f;
                    isMetal = true;
                }
                else
                {
                    MessageBox.Show("How dou you not have a volume ?");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        static AudioEndpointVolume GetDefaultAudioEndpointVolume()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            MMDevice defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return defaultDevice.AudioEndpointVolume;
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = Icon.FromHandle(Properties.Resources.high_volume.GetHicon()),
                Visible = true
            };
            GetVolume();

            contextMenu = new ContextMenuStrip();

            ToolStripMenuItem menuItem1 = new ToolStripMenuItem("Metal Mode", Properties.Resources.sabaton_s_white);
            ToolStripMenuItem menuItem2 = new ToolStripMenuItem("Standard Mode", Properties.Resources.high_volume);
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Exit", Properties.Resources.x_exit);

            menuItem1.Click += MenuItem1_Click;
            menuItem2.Click += MenuItem2_Click;
            exitMenuItem.Click += ExitMenuItem_Click;

            contextMenu.Items.Add(menuItem1);
            contextMenu.Items.Add(menuItem2);
            contextMenu.Items.Add(exitMenuItem);

            notifyIcon.ContextMenuStrip = contextMenu;

            notifyIcon.Text = "Click to toggle modes | Current: Standard";

            contextMenu.LostFocus += ContextMenu_LostFocus;
            notifyIcon.MouseClick += NotifyIcon_Click;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            if (isMetal == false)
            {
                MenuItem1_Click(sender, e);
                isMetal = true;
                notifyIcon.Text = "Click to toggle modes | Current: Metal";
                notifyIcon.Icon = Icon.FromHandle(Properties.Resources.Sabaton_S_Logo.GetHicon());
                return;
            }
            else
            {
                MenuItem2_Click(sender, e);
                isMetal = false;
                notifyIcon.Text = "Click to toggle modes | Current: Standard";
                notifyIcon.Icon = Icon.FromHandle(Properties.Resources.high_volume.GetHicon());
                return;
            }
        }

        private void MenuItem1_Click(object sender, EventArgs e)
        {
            SetVolume(0.5f);
        }

        private void MenuItem2_Click(object sender, EventArgs e)
        {
            SetVolume(1f);
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }
        static void SetVolume(float volume)
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            if (volume < 0.0f)
                volume = 0.0f;
            else if (volume > 1.0f)
                volume = 1.0f;

            device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
        }

        private void ContextMenu_LostFocus(object sender, EventArgs e)
        {
            contextMenu.Hide();
        }
    }
}
