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
using System.Net;
using SimpleTcp;

namespace server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        SimpleTcpServer server;
        string ipPort = "127.0.0.1:15231";
        string data = "";
        List<string> connectedUsernames = new List<string>();
        List<string> connectedIpPorts = new List<string>();  // these two arrays are parallel and they keep connected clients infos such as their username and ip:port
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            server = new SimpleTcpServer(ipPort);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;

        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                data = Encoding.UTF8.GetString(e.Data).ToString();
                if (data.Length >= 25 && data.Substring(0, 25) == "#l*o*g*g*e*d*i*n*u*s*e*r#")
                {
                    connectedUsernames.Add(data.Substring(25));
                }
                else
                {
                    string destUser = "";
                    int i = 0;
                    bool isDestOnline = false;
                    int index = connectedIpPorts.FindIndex(a => a.Contains(e.IpPort));
                    string destIP = e.IpPort.ToString();
                    for (i = 0; i < data.Length && data[i].ToString() != ":"; i++)
                    {
                        destUser += data[i].ToString();
                    }
                    data = connectedUsernames[index] + data.Substring(i);
                    foreach (string user in connectedUsernames)
                    {
                        if (user == destUser)
                        {
                            isDestOnline = true;
                            index = connectedUsernames.FindIndex(a => a.Contains(user));
                            destIP = connectedIpPorts[index];

                        }
                    }
                    if (!isDestOnline)
                    {
                        data = $" server: user {destUser} is offline and won't receive your messages.";
                    }
                    if (server.IsListening)
                    {
                        if (!string.IsNullOrEmpty(destIP) && !string.IsNullOrEmpty(data))
                        {
                            server.Send(destIP, data);
                        }
                    }
                }
            }));
            
        }

        private void Events_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                int index = connectedIpPorts.FindIndex(a => a.Contains(e.IpPort));
                connectedIpPorts.Remove(e.IpPort);
                connectedUsernames.Remove(connectedUsernames[index]);
            }));
            
        }

        private void Events_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                connectedIpPorts.Add(e.IpPort);
                if (server.IsListening)
                {
                    server.Send("server: you are connected to server", e.IpPort);
                }
            }));
            
        }


        private void btnCnct_Click(object sender, RoutedEventArgs e)
        {
            server.Start();
            MessageBox.Show("server started.", "server status", MessageBoxButton.OK, MessageBoxImage.Information);
            this.WindowState = WindowState.Minimized;
        }
    }
}
