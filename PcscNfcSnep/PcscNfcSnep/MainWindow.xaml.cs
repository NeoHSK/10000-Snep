﻿using Newtonsoft.Json;
using PcscNfcSnep.NDEF;
using PcscNfcSnep.PCSC.NFC;
using PcscNfcSnep.POC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IOStorage;

namespace PcscNfcSnep
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReaderContext readerContext = new ReaderContext();

        List<MeasurementMessage> measurementMessages = new List<MeasurementMessage>();

        MeasurementMessage measurementMessage = new MeasurementMessage();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            readerContext.InitializeReader();

            byte[] temp = null;

            readerContext.ReaderControl(SNEP.ECommand.Start, temp);

            readerContext.ReaderControl(SNEP.ECommand.PutTimeout, temp);

            readerContext.ReaderControl(SNEP.ECommand.RecieveTimeout, temp);

            ResultBlock.Text = "Start";
        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            //One by one           
            //NdefMessage ndefRecords = new NdefMessage() { new NdefRecord(new DeviceInfoMessage()) };

            //ndefRecords.Clear();

            //Continuous
            //NdefRecord ndef = new NdefRecord(new DeviceInfoMessage());
            //NdefRecord ndef2 = new NdefRecord(new TimeSyncMessage());
            //ndefRecords = new NdefMessage() { ndef , ndef2 };

            byte[] temp = null;

            //readerContext.ReaderControl(SNEP.ECommand.Stop, temp);

            //ResultBlock.Text = "Stop";

            string mDir = "D:\\NIPRO_NFC";
            string relPath = "RESULT";
            string fullPath = mDir + System.IO.Path.AltDirectorySeparatorChar+ relPath;

            string mDir2 = "D:\\NIPRO_NFC\\";
            string relPath2 = "RESULT";
            string fullPath2 = mDir2 + System.IO.Path.DirectorySeparatorChar + relPath2;

            string[] paths = { "C:\\NIPRO_P2P", "RESULT\\", "20210308" };

            string output;

            output = System.IO.Path.Combine(mDir2,relPath2);
            output = System.IO.Path.Combine(paths);

            ResultBlock.Text = output;
        }

        private void Device_Info_Button_Click(object sender, RoutedEventArgs e)
        {
            //TODO NDEF Message 를 만들어서 rawData로 변경하여 Reader에 put하도록 변경
            
            DeviceInfoMessage deviceInfoMessage = new DeviceInfoMessage();

            readerContext.ReaderPut(SNEP.ECommand.Put, deviceInfoMessage);

            //TODO 예외처리 payload가 0 일때
            deviceInfoMessage.ResponseMessage(readerContext.ReaderRecieve()[0].Payload);
            ResultBlock.Text = deviceInfoMessage.ToString();
        }

        private void TimeSync_Button_Click(object sender, RoutedEventArgs e)
        {
            TimeSyncMessage timeSyncMessage = new TimeSyncMessage();
            TimeSyncMessage recievetimeSyncMessage = new TimeSyncMessage();
            readerContext.ReaderPut(SNEP.ECommand.Put, timeSyncMessage);

            recievetimeSyncMessage.Day = 0;
            recievetimeSyncMessage.Dst = 0;
            recievetimeSyncMessage.Month = 0;
            recievetimeSyncMessage.Year = 0;
            recievetimeSyncMessage.Second = 0;
            recievetimeSyncMessage.Minute = 0;

            recievetimeSyncMessage.ResponseMessage(readerContext.ReaderRecieve()[0].Payload);

            ResultBlock.Text = recievetimeSyncMessage.ToString();
        }

        private void Remaining_Button_Click(object sender, RoutedEventArgs e)
        {
            uint lastSequenceNumber = 1;
            uint remainingDataCount;
            MeasurementMessage measurementMessage = new MeasurementMessage();

            readerContext.ReaderPut(SNEP.ECommand.Put, measurementMessage.RequestRemaingCount(lastSequenceNumber));

            remainingDataCount = measurementMessage.ResponseRemaingCount(readerContext.ReaderRecieve()[0].Payload);

            ResultBlock.Text = remainingDataCount.ToString();
        }

        private void Measurement_Button_Click(object sender, RoutedEventArgs e)
        {
            const uint MEASUREMENT_SIZE = 179;

            uint lastSequenceNumber = 1;

            string textBlcok;

            textBlcok = LastSequenceNumber.Text;

            uint intSeqNum;
            uint receiveCnt = 0;

            NdefMessage ndefRecords = new NdefMessage();

            if(uint.TryParse(textBlcok,out intSeqNum))
            {
                readerContext.ReaderPut(SNEP.ECommand.Put, measurementMessage.RequestMeasurementMessage(intSeqNum));

                while (true)
                {
                    receiveCnt++;
                    ndefRecords = readerContext.ReaderRecieve();

                    measurementMessage.ResponseMessage(ndefRecords[0].Payload);

                    if((ndefRecords[0].MessageInfoFlag & NdefRecord.EMessageInfoFlags.ME) == NdefRecord.EMessageInfoFlags.ME)
                            break;
                }

                List <MeasurementMessage> measurementMessages = measurementMessage.GetMeasurementMessages();

                var output = JsonConvert.SerializeObject(measurementMessages);

                StorageManager.FileWrite("JSON.json",output);

                ResultBlock.Text = $"Try - {receiveCnt} \n" +
                    $" Recieved - {measurementMessage.GetMeasurementMessages().Count} \n" +
                    $" Complete \n" + output;
            }

        }
    }
}
