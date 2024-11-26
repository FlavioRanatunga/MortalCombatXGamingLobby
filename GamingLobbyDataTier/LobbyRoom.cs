using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GamingLobbyDataTier
{

    [DataContract]
    public class LobbyRoom
    {
        [DataMember]
        public int roomId { get; set; }

        [DataMember]
        private List<User> usersInRoom;


        private List<Message> _messagesInRoom;
        private int _messageCount;

        [DataMember]
        public string RoomName { get; set; }
      

        public LobbyRoom(string roomName, int roomNumber)
        {
            RoomName = roomName;
            usersInRoom = new List<User>();
            roomId=roomNumber;
            _messageCount = 0;
            _messagesInRoom = new List<Message>();
       

        }

        public int GetRoomId()
        {  return roomId; }


        public void AddMessageToRoom( string username,string message)
        {
            _messageCount++;
            Message newMessage = new Message(message, username,_messageCount);
            _messagesInRoom.Add(newMessage);
        }

        public List<Message> GetMessageList()
        {
               return _messagesInRoom;
        }

        public void AddFileMessageToRoom(string username, string finalDestinationPath)
        {
            _messageCount++;
            Message newMessage = new Message(finalDestinationPath, username, _messageCount,true);
            _messagesInRoom.Add(newMessage);
        }

        public List<User> GetPlayers()
        {
            return usersInRoom;
        }

        public void AddPlayer(User player)
        {

            usersInRoom.Add(player);
            //players.Add(player);
        }

        public void RemovePlayer(User player)
        {
            usersInRoom.Remove(player);
        }

        public List<string> GetPlayerNames()
        {
            List<string> playerNames = new List<string>();
            foreach (var player in usersInRoom)
            {
                playerNames.Add(player.GetUserName());
            }
            return playerNames;
        }





    }
}
