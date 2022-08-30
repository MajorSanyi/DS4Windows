using DS4Windows;

namespace DS4WinWPF.DS4Control.Common
{
    public abstract class OutputDevice
    {
        protected bool connected;

        public abstract string GetDeviceType { get; }

        public abstract void ConvertandSendReport(DS4State state, int device);
        public abstract void Connect();
        public abstract void Disconnect();
        public abstract void ResetState(bool submit = true);

        public abstract void RemoveFeedbacks();

        public abstract void RemoveFeedback(int inIdx);
    }
}
