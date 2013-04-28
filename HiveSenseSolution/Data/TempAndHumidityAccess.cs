using System;
using System.IO;
using Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using HiveSense;
using Microsoft.SPOT;

namespace GadgeteerApp1.Data
{
    public class TempAndHumidityAccess
    {
        //private const string LoggingFolder = @"\SD\TempAndHumidity\";

        private SDCard _sdCard;
        private string _fileName;

        private int _dataCounter = 0;

        private const int C_DATA_READ_COUNT = 10;

        private long _beginSeekPosition = 0;

        private Reporter _reporter;

        GadgeteerApp1.Program _programMain; 

        public TempAndHumidityAccess(SDCard sdCard, DateTime date, GadgeteerApp1.Program p)
        {
            _sdCard = sdCard;
            _fileName = date.ToString("dd.MM.yyyy") + ".txt";
            //_fileName = "test.txt";

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

           
            this._programMain = p; 
        }

        public TempAndHumidity[] GetAll()
        {
            if (!IsSdCardReady())
            {
                return null;
            }

            StorageDevice storageDevice = _sdCard.GetStorageDevice();

            TempAndHumidity[] returnArr = new TempAndHumidity[10];

            using (FileStream fileStream = storageDevice.OpenRead(_fileName))
            using (var fwd = new StreamReader(fileStream))
            {
                fwd.BaseStream.Seek(_beginSeekPosition, 0);
                int i = 0; 
                do
                {
                    var line = fwd.ReadLine();
                    TempAndHumidity tempHumidity = new TempAndHumidity(line);
                    returnArr[i] = tempHumidity;
                    i++;
                    Debug.Print(line); 
                } while (fwd.EndOfStream == false);
            }
            return returnArr; 
        }

        public void Log(TempAndHumidity tempAndHumidity)
        {
            // TODO: If the SD Card isn't available, what should we do?
            if (!IsSdCardReady())
            {
                return; // Do nothing
            }

            StorageDevice storageDevice = _sdCard.GetStorageDevice();


            using (FileStream fileStream = storageDevice.OpenWrite(_fileName))
            using (StreamWriter fwd = new StreamWriter(fileStream))
            {
                if (_dataCounter == 0)
                {
                    _beginSeekPosition = fileStream.Length;
                }
                fwd.BaseStream.Seek(fileStream.Length, 0);
                fwd.WriteLine(tempAndHumidity.ToCsvString());
                fwd.Flush();

                _dataCounter++;
            }

            if (_dataCounter == 10)
            {
                _dataCounter = 0;
                var arr = this.GetAll();
                if (_programMain.WifiWasConnected)
                {
                    if (_reporter == null)
                    {
                        this._reporter = new HiveSense.Reporter("18824523-d961-4f14-922c-6ea2783ffe31");
                    }
                    _reporter.ReportMetrics(arr, "hive1");
                }

            }
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
