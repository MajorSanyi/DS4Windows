namespace DS4WinWPF.DS4Control.ControllerServiceDeviceOptions
{
    public class ControlServiceDeviceOptions
    {
        private readonly DS4DeviceOptions dS4DeviceOpts = new();
        public DS4DeviceOptions DS4DeviceOpts { get => dS4DeviceOpts; }

        private readonly DualSenseDeviceOptions dualSenseOpts = new();
        public DualSenseDeviceOptions DualSenseOpts { get => dualSenseOpts; }

        private readonly SwitchProDeviceOptions switchProDeviceOpts = new SwitchProDeviceOptions();
        public SwitchProDeviceOptions SwitchProDeviceOpts { get => switchProDeviceOpts; }

        private readonly JoyConDeviceOptions joyConDeviceOpts = new();
        public JoyConDeviceOptions JoyConDeviceOpts { get => joyConDeviceOpts; }

        private bool verboseLogMessages;
        public bool VerboseLogMessages { get => verboseLogMessages; set => verboseLogMessages = value; }

        public ControlServiceDeviceOptions()
        {
            // If enabled then DS4Windows shows additional log messages when a gamepad is connected (may be useful to diagnose connection problems).
            // This option is not persistent (ie. not saved into config files), so if enabled then it is reset back to FALSE when DS4Windows is restarted.
            verboseLogMessages = false;
        }
    }
}
