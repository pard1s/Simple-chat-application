using SimpleTcp;
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
using System.Windows.Shapes;

namespace chatAPP
{
    /// <summary>
    /// Interaction logic for Client.xaml
    /// </summary>
    public partial class Client : Window
    {
        string serverIpPort;
        public Client(string myusername, string ipport)
        {
            InitializeComponent();
            serverIpPort = ipport;
            txtMyusername.Text = myusername;
        }
        SimpleTcpClient client;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            client = new SimpleTcpClient(serverIpPort);
            client.Events.Connected += Events_Connected;
            client.Events.Disconnected += Events_Disconnected;
            client.Events.DataReceived += Events_DataReceived;
            try
            {
                client.Connect();
                if (client.IsConnected)
                {
                    string usrnametoserver = "#l*o*g*g*e*d*i*n*u*s*e*r#" + txtMyusername.Text;
                    client.Send(usrnametoserver);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                txtLog.Text += Encoding.UTF8.GetString(e.Data) + Environment.NewLine ;
            }));
            
        }

        private void Events_Disconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                txtLog.Text += $"Server disconnected. {Environment.NewLine}";
            }));
            
        }

        private void Events_Connected(object sender, ClientConnectedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                txtLog.Text += $"connected to the server. {Environment.NewLine}";
            }));
            
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (client.IsConnected)
            {
                if (!string.IsNullOrEmpty(txtMessage.Text))
                {
                    client.Send(txtDestusername.Text + ":" + txtMessage.Text);
                    txtLog.Text += $"{txtMyusername.Text}: {txtMessage.Text}{Environment.NewLine}";
                    txtMessage.Text = string.Empty;
                }
                
            }
        }
    }
}
