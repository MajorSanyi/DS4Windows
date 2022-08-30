using System;

namespace DS4WinWPF.DS4Control.ControllerServiceDeviceOptions
{
    public class JoyConDeviceOptions
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

        public enum LinkMode : ushort
        {
            Split,
            Joined,
        }

        private LinkMode linkedMode = LinkMode.Joined;
        public LinkMode LinkedMode
        {
            get => linkedMode;
            set
            {
                if (linkedMode == value) return;
                linkedMode = value;
            }
        }

        public enum JoinedGyroProvider : ushort
        {
            JoyConL,
            JoyConR,
        }

        private JoinedGyroProvider joinGyroProv = JoinedGyroProvider.JoyConR;
        public JoinedGyroProvider JoinGyroProv
        {
            get => joinGyroProv;
            set
            {
                if (joinGyroProv == value) return;
                joinGyroProv = value;
            }
        }
    }
}
