using DS4Windows;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace DS4WinWPF.DS4Control.DS4OutDevices
{
    class DS4OutDeviceBasic : DS4OutDevice
    {
        public DS4OutDeviceBasic(ViGEmClient client) : base(client)
        {
        }

        public override void ConvertandSendReport(DS4State state, int device)
        {
            if (!connected) return;

            //cont.ResetReport();
            ushort tempButtons = 0;
            DualShock4DPadDirection tempDPad;
            ushort tempSpecial = 0;
            unchecked
            {
                if (state.Share) tempButtons |= DualShock4Button.Share.Value;
                if (state.L3) tempButtons |= DualShock4Button.ThumbLeft.Value;
                if (state.R3) tempButtons |= DualShock4Button.ThumbRight.Value;
                if (state.Options) tempButtons |= DualShock4Button.Options.Value;

                tempDPad = GetDpadDirection(state);

                if (state.L1) tempButtons |= DualShock4Button.ShoulderLeft.Value;
                if (state.R1) tempButtons |= DualShock4Button.ShoulderRight.Value;
                //if (state.L2Btn) tempButtons |= DualShock4Buttons.TriggerLeft;
                //if (state.R2Btn) tempButtons |= DualShock4Buttons.TriggerRight;
                if (state.L2 > 0) tempButtons |= DualShock4Button.TriggerLeft.Value;
                if (state.R2 > 0) tempButtons |= DualShock4Button.TriggerRight.Value;

                if (state.Triangle) tempButtons |= DualShock4Button.Triangle.Value;
                if (state.Circle) tempButtons |= DualShock4Button.Circle.Value;
                if (state.Cross) tempButtons |= DualShock4Button.Cross.Value;
                if (state.Square) tempButtons |= DualShock4Button.Square.Value;
                if (state.PS) tempSpecial |= DualShock4SpecialButton.Ps.Value;
                if (state.OutputTouchButton) tempSpecial |= DualShock4SpecialButton.Touchpad.Value;
                Cont.SetButtonsFull(tempButtons);
                Cont.SetSpecialButtonsFull((byte)tempSpecial);
                Cont.SetDPadDirection(tempDPad);
                //report.Buttons = (ushort)tempButtons;
                //report.SpecialButtons = (byte)tempSpecial;
            }

            Cont.LeftTrigger = state.L2;
            Cont.RightTrigger = state.R2;

            SetTriggerAxis(state, Global.GetSASteeringWheelEmulationAxis(device));

            Cont.SubmitReport();
        }

        public override void ResetState(bool submit = true)
        {
            Cont.ResetReport();
            if (submit)
            {
                Cont.SubmitReport();
            }
        }

        private DualShock4DPadDirection GetDpadDirection(DS4State state)
        {
            return state switch
            {
                { DpadUp: true, DpadRight: true } => DualShock4DPadDirection.Northeast,
                { DpadUp: true, DpadLeft: true } => DualShock4DPadDirection.Northeast,
                { DpadUp: true } => DualShock4DPadDirection.North,
                { DpadRight: true, DpadDown: true } => DualShock4DPadDirection.Southeast,
                { DpadRight: true } => DualShock4DPadDirection.East,
                { DpadDown: true, DpadLeft: true } => DualShock4DPadDirection.East,
                { DpadDown: true } => DualShock4DPadDirection.East,
                { DpadLeft: true } => DualShock4DPadDirection.East,
                _ => DualShock4DPadDirection.None,
            };
        }
        private void SetTriggerAxis(DS4State state, SASteeringWheelEmulationAxisType steeringWheelMappedAxis)
        {
            switch (steeringWheelMappedAxis)
            {
                case SASteeringWheelEmulationAxisType.None:
                    Cont.LeftThumbX = state.LX;
                    Cont.LeftThumbY = state.LY;
                    Cont.RightThumbX = state.RX;
                    Cont.RightThumbY = state.RY;
                    break;

                case SASteeringWheelEmulationAxisType.LX:
                    Cont.LeftThumbX = (byte)state.SASteeringWheelEmulationUnit;
                    Cont.LeftThumbY = state.LY;
                    Cont.RightThumbX = state.RX;
                    Cont.RightThumbY = state.RY;
                    break;

                case SASteeringWheelEmulationAxisType.LY:
                    Cont.LeftThumbX = state.LX;
                    Cont.LeftThumbY = (byte)state.SASteeringWheelEmulationUnit;
                    Cont.RightThumbX = state.RX;
                    Cont.RightThumbY = state.RY;
                    break;

                case SASteeringWheelEmulationAxisType.RX:
                    Cont.LeftThumbX = state.LX;
                    Cont.LeftThumbY = state.LY;
                    Cont.RightThumbX = (byte)state.SASteeringWheelEmulationUnit;
                    Cont.RightThumbY = state.RY;
                    break;

                case SASteeringWheelEmulationAxisType.RY:
                    Cont.LeftThumbX = state.LX;
                    Cont.LeftThumbY = state.LY;
                    Cont.RightThumbX = state.RX;
                    Cont.RightThumbY = (byte)state.SASteeringWheelEmulationUnit;
                    break;

                case SASteeringWheelEmulationAxisType.L2R2:
                    Cont.LeftTrigger = Cont.RightTrigger = 0;
                    if (state.SASteeringWheelEmulationUnit >= 0) Cont.LeftTrigger = (byte)state.SASteeringWheelEmulationUnit;
                    else Cont.RightTrigger = (byte)state.SASteeringWheelEmulationUnit;
                    goto case SASteeringWheelEmulationAxisType.None;

                case SASteeringWheelEmulationAxisType.VJoy1X:
                case SASteeringWheelEmulationAxisType.VJoy2X:
                    DS4Windows.VJoyFeeder.vJoyFeeder.FeedAxisValue(state.SASteeringWheelEmulationUnit, ((uint)steeringWheelMappedAxis - (uint)SASteeringWheelEmulationAxisType.VJoy1X) / 3 + 1, DS4Windows.VJoyFeeder.HID_USAGES.HID_USAGE_X);
                    goto case SASteeringWheelEmulationAxisType.None;

                case SASteeringWheelEmulationAxisType.VJoy1Y:
                case SASteeringWheelEmulationAxisType.VJoy2Y:
                    DS4Windows.VJoyFeeder.vJoyFeeder.FeedAxisValue(state.SASteeringWheelEmulationUnit, ((uint)steeringWheelMappedAxis - (uint)SASteeringWheelEmulationAxisType.VJoy1X) / 3 + 1, DS4Windows.VJoyFeeder.HID_USAGES.HID_USAGE_Y);
                    goto case SASteeringWheelEmulationAxisType.None;

                case SASteeringWheelEmulationAxisType.VJoy1Z:
                case SASteeringWheelEmulationAxisType.VJoy2Z:
                    DS4Windows.VJoyFeeder.vJoyFeeder.FeedAxisValue(state.SASteeringWheelEmulationUnit, ((uint)steeringWheelMappedAxis - (uint)SASteeringWheelEmulationAxisType.VJoy1X) / 3 + 1, DS4Windows.VJoyFeeder.HID_USAGES.HID_USAGE_Z);
                    goto case SASteeringWheelEmulationAxisType.None;

                default:
                    // Should never come here but just in case use the NONE case as default handler....
                    goto case SASteeringWheelEmulationAxisType.None;
            }
        }
    }
}
