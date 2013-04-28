using System;
using System.Collections;
using Microsoft.SPOT;
using Gadgeteer.Modules.Seeed;

namespace GadgeteerApp1.Cellular
{
    public class CellularObject
    {
        CellularRadio _cellularRadio;
        bool _canSend = false;
        bool _isIntialised = false;
        bool _isInitialising = false;
        ArrayList _outgoingMessages = new ArrayList();

        public CellularObject(CellularRadio celluarRadio)
        {
            _cellularRadio = celluarRadio;
            _cellularRadio.DebugPrintEnabled = true;
            _cellularRadio.ModuleInitialized += new CellularRadio.ModuleInitializedHandler(_cellularRadio_ModuleInitialized);
        }

        void _cellularRadio_ModuleInitialized(CellularRadio sender)
        {
            _canSend = true;
            _isInitialising = false;
            _isIntialised = true;

            StartSending();
        }

        private void StartSending()
        {
            if (_canSend)
            {
                foreach (string message in GetMessagesToSend())
                    _cellularRadio.SendSms("APhoneNumber", message);

                TurnOffCellular();
            }
        }

        private void TurnOffCellular()
        {
            _cellularRadio.PowerOff();
            _isIntialised = false;
            _isInitialising = false;
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
            _isInitialising = true;
            _cellularRadio.PowerOn(40);
        }

        public void SendSms(string message)
        {
            if (!_isIntialised || !_isInitialising)
                InitialiseCelluar();

            _outgoingMessages.Add(message);
        }

        public void RequestTheTime()
        {
            _cellularRadio.RetrieveClock();
        }
    }
}
