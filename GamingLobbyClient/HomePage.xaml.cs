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
using IGamingLobbyServer;
using GamingLobbyDataTier;
using System.ServiceModel;
using System.Threading;

namespace GamingLobbyClient
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Window
    {
        GamingLobbyServerInterface foob;
        private CancellationTokenSource cancelSrc;
        private string username;

        public HomePage(string username, GamingLobbyServerInterface server)
        {
            InitializeComponent();
            this.username = username;
            this.foob = server;
            DisplayLobbyRooms();
            CheckChannelStatus();
            BeginBackgroundPolling();
        }

        private void LobbyNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "Enter Lobby Name")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void LobbyNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Enter Lobby Name";
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void DisplayLobbyRooms()
        {
            CheckChannelStatus();
            List<LobbyRoom> rooms = new List<LobbyRoom>();
            try
            {
                rooms = foob.GetLobbyRooms();
                
                foreach (var room in rooms)
                {
                    LobbyRoomsListBox.Items.Add(room.RoomName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching lobby rooms: {ex.Message}");
            }

        }

        private void CheckChannelStatus()
        {
            var state = ((ICommunicationObject)foob).State;
            //Console.WriteLine($"Channel state: {state} HomePage");
        }

        private void CreateLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var roomName = LobbyNameTextBox.Text;
                if (string.IsNullOrWhiteSpace(roomName) || roomName == "Enter Lobby Name")
                {
                    MessageBox.Show("Please enter a valid lobby name");
                    return;
                }
                else
                {
                    if (foob.CreateLobbyRoom(roomName))
                    {
                        RefreshLobbyRoomsList();
                        MessageBox.Show("Lobby room created successfully");
                    }
                    else
                    {
                        MessageBox.Show("Lobby room already exists");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating lobby room: {ex.Message}");
            }
        }

        private void RefreshLobbyRoomsList()
        {
            try
            {
                LobbyRoomsListBox.Items.Clear(); // Clear the existing items
                DisplayLobbyRooms();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing lobby rooms: {ex.Message}");
            }
        }

        private void BeginBackgroundPolling()
        {
            cancelSrc = new CancellationTokenSource();
            var token = cancelSrc.Token;

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RefreshLobbyRoomsList();
                    });
                    await Task.Delay(10000);

                }
            }, token);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            cancelSrc.Cancel();
        }

        private void JoinLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            if (LobbyRoomsListBox.SelectedItem == null || string.IsNullOrEmpty(LobbyRoomsListBox.SelectedItem.ToString()))
            {
                MessageBox.Show("Please select a lobby room to join");
                //return;
            }
            else
            {
                string selectedLobbyRoomName = LobbyRoomsListBox.SelectedItem.ToString();
                foob.JoinLobbyRoom(selectedLobbyRoomName, username);
                MessageBox.Show($"Joining lobby room: {selectedLobbyRoomName}");
                cancelSrc.Cancel();
                ChatRoomWindow chatRoomWindow = new ChatRoomWindow(username, selectedLobbyRoomName,foob);
                chatRoomWindow.Show();
                this.Close();
            }

        } 

        private void LobbyRoomsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LobbyRoomsListBox.SelectedItem != null)
            {
                string selectedLobbyRoomName = LobbyRoomsListBox.SelectedItem.ToString();
                Console.WriteLine($"Selected lobby room: {selectedLobbyRoomName}");
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            foob.Logout(username);
            MessageBox.Show("Logged out successfully");
            cancelSrc.Cancel();
            this.Close();
        }
    }
}
