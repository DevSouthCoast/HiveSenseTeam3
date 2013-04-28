using System;
using System.Collections;
using Microsoft.SPOT;
using Gadgeteer.Modules.Seeed;

namespace GadgeteerApp1.Cellular
{
    public class CellularObject
    {
        CellularRadio _cellularRadio;
        bool _isIntialised = false;
        bool _isInitialising = false;
        ArrayList _outgoingMessages = new ArrayList();
        bool _coldStart = true;

        public CellularObject(CellularRadio celluarRadio)
        {
            _cellularRadio = celluarRadio;
            _cellularRadio.DebugPrintEnabled = true;
            _cellularRadio.ModuleInitialized += new CellularRadio.ModuleInitializedHandler(_cellularRadio_ModuleInitialized);
        }

        void _cellularRadio_ModuleInitialized(CellularRadio sender)
        {
            Debug.Print("Cellular ready...");
            _isInitialising = false;
            _isIntialised = true;

            StartSending();
        }

        private void StartSending()
        {
            if (_isIntialised)
            {
                Debug.Print("Sending messages...");

                foreach (string message in GetMessagesToSend())
                    _cellularRadio.SendSms("!!!ANUMBER!!!", message);

                Debug.Print("All messages sent...");
                TurnOffCellular();
            }
        }

        private void TurnOffCellular()
        {
            Debug.Print("Turning off cellular...");
            _cellularRadio.PowerOff();
            _isIntialised = false;
            _isInitialising = false;
            Debug.Print("Cellular off...");
        }

        IEnumerable GetMessagesToSend()
        {
            while (_outgoingMessages.Count > 0)
            {
                string result = _outgoingMessages[0] as string;

                _outgoingMessages.Remove(result);

                yield return result;
            }
        }

        void InitialiseCelluar()
        {
            Debug.Print("Starting cellular initialisation...");
            _isInitialising = true;

            if (_coldStart)
            {
                Debug.Print("This will take 60 seconds...");
                _cellularRadio.PowerOn(60);
            }
            else
                _cellularRadio.PowerOn();
        }

        public void SendSms(string message)
        {
            if (!_isIntialised || !_isInitialising)
                InitialiseCelluar();

            _outgoingMessages.Add(message);
        }
    }
}
