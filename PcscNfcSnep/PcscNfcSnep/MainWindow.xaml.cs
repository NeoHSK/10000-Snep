using PcscNfcSnep.NDEF;
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
            using()
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}
