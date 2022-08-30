using DS4WinWPF.DS4Control.DS4OutDevices;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS4Windows
{
    class DS4OutDeviceExt : DS4OutDevice
    {
        private readonly byte[] rawOutReportEx = new byte[63];
        private DS4_REPORT_EX outDS4Report;

        public DS4OutDeviceExt(ViGEmClient client) : base(client)
        {
        }

        public override unsafe void ConvertandSendReport(DS4State state, int device)
        {
            if (!connected) return;

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

                outDS4Report.wButtons = tempButtons;
                // Frame counter is high 6 bits. Low 2 bits is for extra buttons (PS, TP Click)
                outDS4Report.bSpecial = (byte)(tempSpecial | (state.FrameCounter << 2));
                outDS4Report.wButtons |= tempDPad.Value;
            }

            outDS4Report.bTriggerL = state.L2;
            outDS4Report.bTriggerR = state.R2;

            SASteeringWheelEmulationAxisType steeringWheelMappedAxis = Global.GetSASteeringWheelEmulationAxis(device);
            MapAllAxis(state, steeringWheelMappedAxis);

            // Only worry about mapping 1 touch packet
            outDS4Report.bTouchPacketsN = 1;
            outDS4Report.sCurrentTouch.bPacketCounter = state.TouchPacketCounter;
            outDS4Report.sCurrentTouch.bIsUpTrackingNum1 = state.TrackPadTouch0.RawTrackingNum;
            outDS4Report.sCurrentTouch.bTouchData1[0] = (byte)(state.TrackPadTouch0.X & 0xFF);
            outDS4Report.sCurrentTouch.bTouchData1[1] = (byte)((state.TrackPadTouch0.X >> 8) & 0x0F | (state.TrackPadTouch0.Y << 4) & 0xF0);
            outDS4Report.sCurrentTouch.bTouchData1[2] = (byte)(state.TrackPadTouch0.Y >> 4);

            outDS4Report.sCurrentTouch.bIsUpTrackingNum2 = state.TrackPadTouch1.RawTrackingNum;
            outDS4Report.sCurrentTouch.bTouchData2[0] = (byte)(state.TrackPadTouch1.X & 0xFF);
            outDS4Report.sCurrentTouch.bTouchData2[1] = (byte)((state.TrackPadTouch1.X >> 8) & 0x0F | (state.TrackPadTouch1.Y << 4) & 0xF0);
            outDS4Report.sCurrentTouch.bTouchData2[2] = (byte)(state.TrackPadTouch1.Y >> 4);

            // Flip some coordinates back to DS4 device coordinate system
            //outDS4Report.wGyroX = (short)-state.Motion.gyroYawFull;
            //outDS4Report.wGyroY = (short)state.Motion.gyroPitchFull;
            outDS4Report.wGyroX = (short)state.Motion.gyroPitchFull;
            outDS4Report.wGyroY = (short)-state.Motion.gyroYawFull;
            outDS4Report.wGyroZ = (short)-state.Motion.gyroRollFull;
            outDS4Report.wAccelX = (short)-state.Motion.accelXFull;
            outDS4Report.wAccelY = (short)-state.Motion.accelYFull;
            outDS4Report.wAccelZ = (short)state.Motion.accelZFull;

            // USB DS4 v.1 battery level range is [0-11]
            outDS4Report.bBatteryLvlSpecial = (byte)(state.Battery / 11);

            outDS4Report.wTimestamp = state.ds4Timestamp;

            DS4OutDeviceExtras.CopyBytes(ref outDS4Report, rawOutReportEx);
            //Console.WriteLine("TEST: {0}, {1} {2}", outDS4Report.wGyroX, rawOutReportEx[12], rawOutReportEx[13]);
            //Console.WriteLine("OUTPUT: {0}", string.Join(", ", rawOutReportEx));
            Cont.SubmitRawReport(rawOutReportEx);
        }

        public override void ResetState(bool submit = true)
        {
            outDS4Report = default;
            outDS4Report.wButtons &= unchecked((ushort)~0X0F);
            outDS4Report.wButtons |= 0x08;
            outDS4Report.bThumbLX = 0x80;
            outDS4Report.bThumbLY = 0x80;
            outDS4Report.bThumbRX = 0x80;
            outDS4Report.bThumbRY = 0x80;
            DS4OutDeviceExtras.CopyBytes(ref outDS4Report, rawOutReportEx);

            if (submit)
            {
                Cont.SubmitRawReport(rawOutReportEx);
            }
        }

        protected override void MapAllAxis(DS4State state, SASteeringWheelEmulationAxisType steeringWheelMappedAxis)
        {
            DS4AxisDto dS4AxisDto = SetAllAxis(state, steeringWheelMappedAxis);
            if (dS4AxisDto.LeftTrigger != null)
                outDS4Report.bTriggerL = (byte)dS4AxisDto.LeftTrigger;
            if (dS4AxisDto.RightTrigger != null)
                outDS4Report.bTriggerR = (byte)dS4AxisDto.RightTrigger;
            outDS4Report.bThumbLX = dS4AxisDto.LX;
            outDS4Report.bThumbLY = dS4AxisDto.LY;
            outDS4Report.bThumbRX = dS4AxisDto.RX;
            outDS4Report.bThumbRY = dS4AxisDto.RY;
        }
    }
}
