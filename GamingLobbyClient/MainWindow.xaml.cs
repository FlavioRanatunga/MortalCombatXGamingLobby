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
using IGamingLobbyServer;
using System.ServiceModel;
using GamingLobbyDataTier;
using System.Threading;

namespace GamingLobbyClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GamingLobbyServerInterface foob;

        public MainWindow()
        {
            InitializeComponent();

            ChannelFactory<GamingLobbyServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            //Set the URL and create the connection!
            string URL = "net.tcp://localhost:8100/MortalCombatXLobbyServer";
            foobFactory = new ChannelFactory<GamingLobbyServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();

        }

     

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {

           String username = UsernameBox.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter a valid username.");
                return;  
            }

            var userExists = await Task.Run(()=>foob.Login(username));
            if (userExists)
            {
                MessageBox.Show($"Welcome {username}");
                Console.WriteLine($"User {username} logged in");
                HomePage homePage = new HomePage(username, foob);
                homePage.Show();
                CheckChannelStatus();
                this.Close();
            }
            else
            {
                MessageBox.Show("User already exists");
            }
           

        }

        private void CheckChannelStatus()
        {
            var state = ((ICommunicationObject)foob).State;
            Console.WriteLine($"Channel state: {state}, Mainwindow");
        }
    }
}
