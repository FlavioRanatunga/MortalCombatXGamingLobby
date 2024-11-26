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
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace GamingLobbyClient
{
    /// <summary>
    /// Interaction logic for ChatRoomWindow.xaml
    /// </summary>
    public partial class ChatRoomWindow : Window
    {
        private GamingLobbyServerInterface gamingLobbyServer;
        private bool isListening;
        private Thread roomMessageListenerThread;
        public ObservableCollection<Message> Messages;
        public ObservableCollection<User> RoomMembers { get; private set; }
        public int latestMessageId;
        string username;
        string roomName;
        public ChatRoomWindow(String username, string roomName,GamingLobbyServerInterface gamingLobbyServer)
        {
            DataContext = this;
            Messages = new ObservableCollection<Message>();
            RoomMembers = new ObservableCollection<User>();
            InitializeComponent();
            this.gamingLobbyServer = gamingLobbyServer;
            isListening = true;
            latestMessageId = 0;
            ListBoxMembers.ItemsSource = RoomMembers;
            this.username = username;
            string usernameLableContent = $"User: {username}";
            UserNameLabel.Content=usernameLableContent;
            
            this.roomName = roomName;
            string roomLabelContent= "You're in Lobby Room: "+ roomName;
            RoomNameLabel.Content = roomLabelContent;
            roomMessageListenerThread = new Thread(new ThreadStart(ListenForRoomMessages));
            roomMessageListenerThread.Start();

            Thread roomMembersPollingThread = new Thread(new ThreadStart(PollForRoomMembers));
            roomMembersPollingThread.Start();
            LoadRoomMembers();


        }

        private void LoadRoomMembers()
        {
            try
            {
                var roomId = gamingLobbyServer.GetRoomIdByUsername(roomName);
                //Console.WriteLine("Fetching members for Room ID: " + roomId);
                var members = gamingLobbyServer.GetRoomUsers(roomId);

                RoomMembers.Clear();
                foreach (var member in members)
                {
                    Debug.WriteLine($"Adding member: {member.GetUserName()}");
                    RoomMembers.Add(member);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading room members: {ex.Message}");
            }
        }

        private void ListBoxMembers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxMembers.SelectedItem is User selectedUser)
            {
                string clickedUsername = selectedUser.GetUserName();
                if (clickedUsername == username)
                {
                    // Do nothing if the user clicks their own username
                    return;
                }
                MessageBox.Show($"Clicked on username: {clickedUsername}");
                PMRoomWindow pMRoomWindow = new PMRoomWindow(username, clickedUsername, gamingLobbyServer);
                pMRoomWindow.Show();
            }
        }

        private void PollForRoomMembers()
        {
            while (isListening)
            {
                try
                {
                    var roomId = gamingLobbyServer.GetRoomIdByUsername(roomName);
                    var members = gamingLobbyServer.GetRoomUsers(roomId);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        RoomMembers.Clear();
                        foreach (var member in members)
                        {
                            RoomMembers.Add(member);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Error polling room members: {ex.Message}");
                    });
                }

                Thread.Sleep(5000); 
            }
        }

        private void ListenForRoomMessages()
        {
            while (isListening)
            {
                var roomId= gamingLobbyServer.GetRoomIdByUsername(roomName);
                
                 var newMessages = gamingLobbyServer.GetRoomMessages(roomId,latestMessageId);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (newMessages != null && newMessages.Any())
                    {

                        foreach (var message in newMessages)
                        {
                            if (message.IsFile)
                            {                     
                                Hyperlink fileLink = new Hyperlink
                                {
                                    NavigateUri = new Uri(message.MessageText)
                                };

                                fileLink.Inlines.Add(System.IO.Path.GetFileName(message.MessageText));
                                fileLink.RequestNavigate += (sender, e) =>
                                {
                                    Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                                };

                                Paragraph paragraph = new Paragraph(fileLink);
                                RichTextBox fileMessageRichTextBox = new RichTextBox();
                                fileMessageRichTextBox.Document.Blocks.Add(paragraph);
                                TextBlock textBlock = new TextBlock();
                                textBlock.Inlines.Add(message.SenderUserName+":  ");

                                textBlock.Inlines.Add(fileLink);
                                textBlock.Inlines.Add(new LineBreak());
                                textBlock.Inlines.Add(message.MessageTimeStamp.ToString("hh:mm tt"));
                                textBlock.Inlines.Add(new LineBreak());

                                ListBoxMessages.Items.Add(textBlock);

                            }
                            else
                            {
                                Debug.WriteLine($"Message ID: {message.MessageId}, Message Text: {message.MessageText}");
                               
                                TextBlock textBlock = new TextBlock();
                                Run messageTextRun = new Run(message.SenderUserName+":  ");
                                messageTextRun.Foreground = new SolidColorBrush(Colors.Blue); 
                                textBlock.Inlines.Add(messageTextRun);
                                textBlock.Inlines.Add(message.MessageText);
                                textBlock.Inlines.Add(new LineBreak());
                                Debug.WriteLine(message.MessageTimeStamp);
                                textBlock.Inlines.Add(message.MessageTimeStamp.ToString("hh:mm tt"));
                                textBlock.Inlines.Add(new LineBreak());

                                ListBoxMessages.Items.Add(textBlock);
                                
                            }


                            

                        }
                        var latestMessage = newMessages.OrderByDescending(m => m.MessageId).FirstOrDefault();
                        if (latestMessage != null)
                        {
                            latestMessageId = latestMessage.MessageId;
                        }

                    }
                });

                Thread.Sleep(1000);
            }
        }

        private void SendMessageBox_Click(object sender, RoutedEventArgs e)
        {
            String message = InputMessageBox.Text;
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Please enter a message.");
                return;  
            }
            var roomId = gamingLobbyServer.GetRoomIdByUsername(roomName);
            gamingLobbyServer.SendRoomMessage(roomId, username, message);
            InputMessageBox.Text = "";
        }

        private void MessageListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private  void FileUploadButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog= new OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|Text Files (*.txt)|*.txt";


            if (openFileDialog.ShowDialog() == true){
                var roomId = gamingLobbyServer.GetRoomIdByUsername(roomName);
                gamingLobbyServer.UploadFile(username, roomId, openFileDialog.FileName);
            }
        }

        private void Username_Click(object sender, RoutedEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            if (hyperlink != null)
            {
                string clickedUsername = ((Run)hyperlink.Inlines.FirstInline).Text;
                if (clickedUsername == username)
                {
                    // Do nothing if the user clicks their own username
                    return;
                }
                MessageBox.Show($"Clicked on username: {clickedUsername}");
                PMRoomWindow pMRoomWindow = new PMRoomWindow(username, clickedUsername, gamingLobbyServer);
                pMRoomWindow.Show();

                this.Close();
            }
        }

        private void LeaveRoomButton_Click(object sender, RoutedEventArgs e)
        {
            gamingLobbyServer.LeaveLobbyRoom(roomName,username);
            MessageBox.Show("You left Lobby Room: " + roomName);
            //isListening = false;
            //roomMessageListenerThread.Join();
            HomePage homePage = new HomePage(username, gamingLobbyServer);
            homePage.Show();
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            isListening = false;
            if (roomMessageListenerThread != null && roomMessageListenerThread.IsAlive)
            {
                roomMessageListenerThread.Join();
            }
        }

    }
}
