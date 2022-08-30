﻿using DS4Windows;
using System.Collections.Generic;
using System.Threading;

namespace DS4WinWPF.DS4Control
{
    public class ControllerSlotManager
    {
        private readonly ReaderWriterLockSlim collectionLocker = new();
        public ReaderWriterLockSlim CollectionLocker { get => collectionLocker; }

        private List<DS4Device> controllerColl;
        public List<DS4Device> ControllerColl { get => controllerColl; set => controllerColl = value; }

        private readonly Dictionary<int, DS4Device> controllerDict;
        private readonly Dictionary<DS4Device, int> reverseControllerDict;
        public Dictionary<int, DS4Device> ControllerDict { get => controllerDict; }
        public Dictionary<DS4Device, int> ReverseControllerDict { get => reverseControllerDict; }

        public ControllerSlotManager()
        {
            controllerColl = new List<DS4Device>();
            controllerDict = new Dictionary<int, DS4Device>();
            reverseControllerDict = new Dictionary<DS4Device, int>();
        }

        public void AddController(DS4Device device, int slotIdx)
        {
            using (WriteLocker locker = new(collectionLocker))
            {
                controllerColl.Add(device);
                controllerDict.Add(slotIdx, device);
                reverseControllerDict.Add(device, slotIdx);
            }
        }

        public void RemoveController(DS4Device device, int slotIdx)
        {
            using (WriteLocker locker = new(collectionLocker))
            {
                controllerColl.Remove(device);
                controllerDict.Remove(slotIdx);
                reverseControllerDict.Remove(device);
            }
        }

        public void ClearControllerList()
        {
            using (WriteLocker locker = new(collectionLocker))
            {
                controllerColl.Clear();
                controllerDict.Clear();
                reverseControllerDict.Clear();
            }
        }
    }
}
