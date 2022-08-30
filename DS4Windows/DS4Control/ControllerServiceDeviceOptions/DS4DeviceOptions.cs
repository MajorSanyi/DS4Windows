using System;

namespace DS4WinWPF.DS4Control.ControllerServiceDeviceOptions
{
    public class DS4DeviceOptions
    {
        private bool enabled = true;
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled == value) return;
                enabled = value;
                EnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler EnabledChanged;
    }
}
