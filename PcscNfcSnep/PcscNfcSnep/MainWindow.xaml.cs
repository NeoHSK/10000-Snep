using PcscNfcSnep.NDEF;
using PcscNfcSnep.PCSC.NFC;
using PcscNfcSnep.POC;
using System;
using System.Collections.Generic;
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

namespace PcscNfcSnep
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReaderContext readerContext = new ReaderContext();
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

            readerContext.ReaderControl(SNEP.ECommand.Stop, temp);

            ResultBlock.Text = "Stop";

        }

        private void Device_Info_Button_Click(object sender, RoutedEventArgs e)
        {
            DeviceInfoMessage deviceInfoMessage = new DeviceInfoMessage();

            readerContext.ReaderPut(SNEP.ECommand.Put, deviceInfoMessage);

            readerContext.ReaderRecieve();

            deviceInfoMessage.Deserialize(readerContext.ReaderRecieve()[0].Payload);
        }

        private void TimeSync_Button_Click(object sender, RoutedEventArgs e)
        {
            TimeSyncMessage timeSyncMessage = new TimeSyncMessage();

            readerContext.ReaderPut(SNEP.ECommand.Put, timeSyncMessage);

            readerContext.ReaderRecieve();

            timeSyncMessage.Deserialize(readerContext.ReaderRecieve()[0].Payload);
        }

        private void Remaining_Button_Click(object sender, RoutedEventArgs e)
        {
            readerContext.ReaderControl(SNEP.ECommand.Put, new MeasurementMessage().RequestRemaingCount());
        }

        private void Measurement_Button_Click(object sender, RoutedEventArgs e)
        {
            const uint MEASUREMENT_SIZE = 179;

            List<MeasurementMessage> measurementMessages = new List<MeasurementMessage>();

            readerContext.ReaderPut(SNEP.ECommand.Put, measurementMessages[0]);

            readerContext.ReaderRecieve();

            var conv = readerContext.ReaderRecieve()[0].Payload;

            var size = conv.Length / MeasurementMessage.MEASUREMENT_MESSAGE_SIZE; 

            measurementMessage.Deserialize(readerContext.ReaderRecieve()[0].Payload);
        }
    }
}
