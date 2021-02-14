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

            NdefRecord ndef = new NdefRecord(new DeviceInfoMessage());

            NdefMessage ndefRecords = new NdefMessage() { ndef};

            var byte1 = ndefRecords.ToByteArray();

            NdefMessage ndefRecords1 = NdefMessage.FromByteArray(byte1);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            readerContext.InitializeReader();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //One by one           
            NdefMessage ndefRecords = new NdefMessage() { new NdefRecord(new DeviceInfoMessage()) };

            ndefRecords.Clear();

            //Continuous
            NdefRecord ndef = new NdefRecord(new DeviceInfoMessage());
            NdefRecord ndef2 = new NdefRecord(new TimeSyncMessage());
            ndefRecords = new NdefMessage() { ndef , ndef2 };

            readerContext.ReaderControl();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}
