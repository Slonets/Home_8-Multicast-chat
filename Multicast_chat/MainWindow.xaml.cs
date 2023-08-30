using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
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
using System.Threading;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Multicast_chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private IPAddress multicastAddress;
        private int port=8001;
        private CancellationTokenSource cancelTokenSource;       
        private CancellationToken cancelToken;
        private string userName;
        public MainWindow()
        {
            InitializeComponent();

            udpClient = new UdpClient();
            multicastAddress = IPAddress.Parse("239.0.0.1");

            textBoxAddress.Text = multicastAddress.ToString();
            textBoxPort.Text = port.ToString();
        }

        private void joinChatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {               

                udpClient.JoinMulticastGroup(multicastAddress);
                
                userName = userNameBox.Text;

                joinChatButton.IsEnabled = false;
                ExitChatButton.IsEnabled = true;

                cancelTokenSource = new CancellationTokenSource();
                Listening(cancelTokenSource.Token);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }

        private void ExitChatButton_Click(object sender, RoutedEventArgs e)
        {
            //Зупинка процесу       

            if (cancelToken!= null && !cancelToken.IsCancellationRequested)
            {
                cancelTokenSource.Cancel();               
            }

            udpClient.DropMulticastGroup(multicastAddress);
            udpClient.Close();
            joinChatButton.IsEnabled = true;
            ExitChatButton.IsEnabled = false;
        }

        // отримання повідомлень з групи
        private async void Listening(CancellationToken cancelTokenF)
        {
            cancelToken = cancelTokenF;            
          
            udpClient.MulticastLoopback = false; // відключаємо отримання власних повідомлень
            while (true)
            {
                try
                {              

                //зупинка, якщо прийшов cancel token
                if (cancelToken != null && cancelToken.IsCancellationRequested)
                {
                    break;
                }

                var result = await udpClient.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);

                 listChat.Items.Add(message);
               
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private void messageBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string? message = messageBox.Text.ToString(); //повідомлення для відправки

            SendMessageAsync(message);

          
        }

        private async void SendMessageAsync(string message)
        {
            // иначе добавляем к сообщению имя пользователя
            message = $"{userName}: {message}";
            byte[] data = Encoding.UTF8.GetBytes(message);
            // и отправляем в группу
            await udpClient.SendAsync(data, data.Length, new IPEndPoint(multicastAddress, port));
        }
    }
}
