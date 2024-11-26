using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using GamingLobbyDataTier;


namespace IGamingLobbyServer
{
    [ServiceContract]
    public interface GamingLobbyServerInterface
    {
        [OperationContract]
         bool Login(String username);

        [OperationContract]
        void SendRoomMessage(int roomId, string username,string message);

        [OperationContract]
        int GetRoomIdByUsername(string roomName);

        [OperationContract]
        List<Message> GetRoomMessages(int roomId,int latestMessageId);

        [OperationContract]
        bool UploadFile(string username,int roomId, string selectedFilePath);

        [OperationContract]
        void SendPrivateMessage(string senderUsername, string recieverUsername, string message);

        [OperationContract]
        List<Message> GetPrivateMessages(string senderUsername, string recieverUsername, int latestMessageId);

        [OperationContract]
        List<LobbyRoom> GetLobbyRooms();

        [OperationContract]
        bool CreateLobbyRoom(string roomName);

        [OperationContract]
        void JoinLobbyRoom(string roomName, string username);

        [OperationContract]
        void LeaveLobbyRoom(string roomName,string username);

        [OperationContract]
        List<User> GetRoomUsers(int roomId);

        [OperationContract]
        void Logout(string username);
    }
}
