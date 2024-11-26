using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IGamingLobbyServer;
using GamingLobbyDataTier;

namespace GamingLobbyClient
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PMRoomWindow : Window
    {
        GamingLobbyServerInterface foob;
        private bool isListening;
        private Thread privateMessageListenerThread;
        public ObservableCollection<Message> Messages { get; private set; }
        public int latestMessageId;
        string sUsername;
        string rUsername;
        public PMRoomWindow(string sUsername, string rUsername, GamingLobbyServerInterface gamingLobbyServer)
        {
            InitializeComponent();
            foob = gamingLobbyServer;
            this.sUsername = sUsername;
            this.rUsername = rUsername;

            UsernameTextBlock.Text = rUsername;
            DataContext = this;
            Messages = new ObservableCollection<Message>();
            latestMessageId = -1;
            MessageListView.ItemsSource = Messages;

            isListening = true;
            privateMessageListenerThread = new Thread(new ThreadStart(ListenForPrivateMessages));
            privateMessageListenerThread.Start();
        }

        private void ListenForPrivateMessages()
        {
            while (isListening)
            {
                var newMessages = foob.GetPrivateMessages(sUsername, rUsername, latestMessageId);
                var newMessages2 = foob.GetPrivateMessages(rUsername, sUsername, latestMessageId);
                var allNewMessages = newMessages.Concat(newMessages2)
                                        .GroupBy(m => m.MessageId) // Group by MessageId to remove duplicates
                                        .Select(g => g.First())
                                        .OrderBy(m => m.MessageId)
                                        .ToList();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (allNewMessages != null && allNewMessages.Any())
                    {
                        foreach (var message in allNewMessages)
                        {
                            Messages.Add(message);
                        }
                        var latestMessage = allNewMessages.Last();
                        latestMessageId = latestMessage.MessageId;
                    }
                });

                Thread.Sleep(1000);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isListening = false;
            if (privateMessageListenerThread != null && privateMessageListenerThread.IsAlive)
            {
                privateMessageListenerThread.Join();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string message = InputMessageBox.Text;
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Please enter a message.");
                return;  
            }
            foob.SendPrivateMessage(sUsername, rUsername, message);
            Console.WriteLine("Sent message to " + rUsername);
        }

        private void LeaveButton_Click(object sender, RoutedEventArgs e)
        {
            isListening = false;
            if (privateMessageListenerThread != null && privateMessageListenerThread.IsAlive)
            {
                privateMessageListenerThread.Join();
            }
            this.Close();
        }
    }
}

