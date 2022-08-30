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

            MapAllAxis(state, Global.GetSASteeringWheelEmulationAxis(device));

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

        protected override void MapAllAxis(DS4State state, SASteeringWheelEmulationAxisType steeringWheelMappedAxis)
        {
            DS4AxisDto dS4AxisDto = SetAllAxis(state, steeringWheelMappedAxis);
            Cont.LeftTrigger = dS4AxisDto.LeftTrigger;
            Cont.RightTrigger = dS4AxisDto.RightTrigger;
            Cont.LeftThumbX = dS4AxisDto.LX;
            Cont.LeftThumbY = dS4AxisDto.LY;
            Cont.RightThumbX = dS4AxisDto.RX;
            Cont.RightThumbY = dS4AxisDto.RY;
        }
    }
}
