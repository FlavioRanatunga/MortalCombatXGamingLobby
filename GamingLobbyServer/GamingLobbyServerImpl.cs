using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGamingLobbyServer;
using System.ServiceModel;
using GamingLobbyDataTier;
using System.IO;
using System.Diagnostics;
using System.ServiceModel.Channels;
using Message = GamingLobbyDataTier.Message;


namespace GamingLobbyServer
{
    [ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    internal class GamingLobbyServerImpl :GamingLobbyServerInterface
    {
       
        UserDatabase userDatabase;
        LobbyRoomDatabase lobbyRoomDatabase;
        PMDatabase pMDatabase;

        public GamingLobbyServerImpl()
        {
            userDatabase= new UserDatabase();
            lobbyRoomDatabase= new LobbyRoomDatabase();
            pMDatabase = new PMDatabase();
            lobbyRoomDatabase.CreateRoom("Room1");
            lobbyRoomDatabase.CreateRoom("Room2");
            Console.WriteLine("Gaming lobby server initiated");

        }


        public bool Login(string username)

        {
            bool authorized =false;

            if (!userDatabase.DoesUserNameExist(username))
            {
                userDatabase.AddUser(username);
                authorized = true;
             }

            return authorized;
        }

        public void SendRoomMessage(int  roomId, string username,string message)
        {

           LobbyRoom room= lobbyRoomDatabase.FindLobbyRoomById(roomId);
          
            room.AddMessageToRoom(username, message);

        }

        public List<Message> GetRoomMessages(int roomId,int latestMessageId)
        {
           
            LobbyRoom room = lobbyRoomDatabase.FindLobbyRoomById(roomId);
           
            List<Message> messageList = room.GetMessageList();

            List<Message> newMessages = messageList
           .Where(m => m.MessageId > latestMessageId) 
           .ToList();

            return newMessages;

        }

        public bool UploadFile( string username, int roomId,string selectedFilePath)
        {
            bool uploaded= false;
            string destinationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DCSharedFiles");
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            string destinationFilePath = Path.Combine(destinationDirectory, Path.GetFileName(selectedFilePath));
            File.Copy(selectedFilePath, destinationFilePath, overwrite: true);
            uploaded = true;
            LobbyRoom room = lobbyRoomDatabase.FindLobbyRoomById(roomId);
            room.AddFileMessageToRoom(username, destinationFilePath);
            return uploaded;

        }


        public void SendPrivateMessage(string senderUsername, string receiverUsername, string message)
        {
            if (userDatabase.GetByUsername(senderUsername) != null && userDatabase.GetByUsername(receiverUsername) != null)
            {
                pMDatabase.AddMessage(senderUsername, receiverUsername, message);
                //pMDatabase.AddMessage(receiverUsername, senderUsername, message);
            }
        }

        public List<Message> GetPrivateMessages(string senderUsername, string receiverUsername, int latestMessageId)
        {
            List<Message> messageList = pMDatabase.GetMessages(senderUsername, receiverUsername);

            List<Message> newMessages = messageList
                .Where(m => m.MessageId > latestMessageId)
                .OrderBy(m => m.MessageId)
                .ToList();

            return newMessages;
        }

        public List<LobbyRoom> GetLobbyRooms()
        {
            try
            {

                List<LobbyRoom> lobbyRooms = lobbyRoomDatabase.GetRooms();
                return lobbyRooms;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLobbyRooms: {ex.Message}");
                throw; // exception is rethrown to inform the client of the error
            }

        }

        public bool CreateLobbyRoom(string roomName)
        {
            bool created = false;
            if (!lobbyRoomDatabase.DoesRoomExist(roomName))
            {
                lobbyRoomDatabase.CreateRoom(roomName);
                created = true;
            }
            return created;
        }

        public void JoinLobbyRoom(string roomName, string username)
        {
           User user = userDatabase.GetByUsername(username);
            Console.WriteLine("current user "+user.GetUserName());
            if (user != null)
            {
                lobbyRoomDatabase.AddUserToRoom(roomName, user);
            }
        }

        public int GetRoomIdByUsername(string roomName)
        {
            bool exists = lobbyRoomDatabase.DoesRoomExist(roomName);

            if (exists) { }
            LobbyRoom room = lobbyRoomDatabase.FindLobbyRoomByName(roomName);
            int roomdId = room.GetRoomId();
            return roomdId;
        }

        public void LeaveLobbyRoom(string roomName, string username) {

            User user = userDatabase.GetByUsername(username);

            lobbyRoomDatabase.RemoveUserFromRoom(roomName, user);
        }

        public List<User> GetRoomUsers(int roomId)
        {
            LobbyRoom room = lobbyRoomDatabase.FindLobbyRoomById(roomId);
            List<User> users = room.GetPlayers();
            return users;
        }

        public void Logout(string username)
        {
            userDatabase.RemoveUser(username);
        }


    }
}
