using System;
using System.IO;
using Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;

namespace Data
{
    public class TempAndHumidityAccess
    {
        //private const string LoggingFolder = @"\SD\TempAndHumidity\";

        private SDCard _sdCard;
        private string _fileName;

        public TempAndHumidityAccess(SDCard sdCard, DateTime date)
        {
            _sdCard = sdCard;
            //_fileName = date.ToString("dd.mm.yyyy") + ".txt";
            _fileName = "test.txt";

            // TODO: Check this later as not sure having this in the constructor is the best idea?

            //if (!Directory.Exists(LoggingFolder))
            //{
            //    Directory.CreateDirectory(LoggingFolder);
            //}

            //sdCard.GetStorageDevice().OpenWrite()

            //if (!File.Exists(LoggingFolder + _fileName))
            //{
            //    File.Create(LoggingFolder + _fileName);
            //}
        }

        public TempAndHumidity GetAll()
        {
            throw new NotImplementedException();
        }

        public void Log(TempAndHumidity tempAndHumidity)
        {
            // TODO: If the SD Card isn't available, what should we do?
            if (!IsSdCardReady())
            {
                return; // Do nothing
            }

            StorageDevice storageDevice = _sdCard.GetStorageDevice();

            FileStream fileStream = storageDevice.OpenWrite(_fileName);

            byte[] data = GetBytes(tempAndHumidity.ToCsvString());

            fileStream.Write(data, 0, 0);

            fileStream.Close();
        }

        static byte[] GetBytes(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        private void CheckOutputFolderExists(DateTime dateTime)
        {
            
        }

        private bool IsSdCardReady()
        {
            if (_sdCard.IsCardInserted && _sdCard.IsCardMounted)
            {
                return true;
            }

            return false;
        }



    }
}
