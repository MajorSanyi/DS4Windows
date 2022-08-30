using System;
using System.Xml;
using DS4Windows.InputDevices;

namespace DS4WinWPF.DS4Control.ControllerServiceDeviceOptions
{
    public class DS4ControllerOptions : ControllerOptionsStore
    {
        private bool copyCatController;
        public bool IsCopyCat
        {
            get => copyCatController;
            set
            {
                if (copyCatController == value) return;
                copyCatController = value;
                IsCopyCatChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler IsCopyCatChanged;

        public DS4ControllerOptions(InputDeviceType deviceType) : base(deviceType)
        {
        }

        public override void PersistSettings(XmlDocument xmlDoc, XmlNode node)
        {
            XmlNode tempOptsNode = node.SelectSingleNode("DS4SupportSettings");
            if (tempOptsNode == null)
            {
                tempOptsNode = xmlDoc.CreateElement("DS4SupportSettings");
            }
            else
            {
                tempOptsNode.RemoveAll();
            }

            XmlNode tempRumbleNode = xmlDoc.CreateElement("Copycat");
            tempRumbleNode.InnerText = copyCatController.ToString();
            tempOptsNode.AppendChild(tempRumbleNode);

            node.AppendChild(tempOptsNode);
        }

        public override void LoadSettings(XmlDocument xmlDoc, XmlNode node)
        {
            XmlNode baseNode = node.SelectSingleNode("DS4SupportSettings");
            if (baseNode != null)
            {
                XmlNode item = baseNode.SelectSingleNode("Copycat");
                if (bool.TryParse(item?.InnerText ?? "", out bool temp))
                {
                    copyCatController = temp;
                }
            }
        }
    }
}
