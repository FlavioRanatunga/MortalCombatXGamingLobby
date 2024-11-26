using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GamingLobbyDataTier
{
    [DataContract]
    public class LobbyRoomDatabase
    {
        [DataMember]
        private List<LobbyRoom> _lobbyRooms;

        private int _roomNumber;
        public LobbyRoomDatabase() { 

            _lobbyRooms = new List<LobbyRoom>();
           _roomNumber = 0;
            Console.WriteLine("Lobby Room Server Initiated");


        }

        public LobbyRoom FindLobbyRoomById(int  id)
        {
            return _lobbyRooms.First(room => room.roomId == id);

        }

        public LobbyRoom FindLobbyRoomByName(string roomName)
        {
            return _lobbyRooms.First(r => r.RoomName == roomName);
        }

        public void CreateRoom(string roomName)
        {
            _roomNumber++;  
            _lobbyRooms.Add(new LobbyRoom(roomName,_roomNumber));
        }

        public void AddUserToRoom(string roomName, User player)
        {
            var room = _lobbyRooms.FirstOrDefault(r => r.RoomName == roomName);
            if (room != null)
            {
                room.AddPlayer(player);
                Console.WriteLine($"User {player.GetUserName()} added to room {roomName}");
            }
            else
            {
                throw new Exception("Room does not exist");
            }
        }

        public void RemoveUserFromRoom(string roomName, User player)
        {
            var room = _lobbyRooms.FirstOrDefault(r => r.RoomName == roomName);
            if (room != null)
            {
                room.RemovePlayer(player);
                Console.WriteLine($"User {player.GetUserName()} removed from room {roomName}");
            }
            else
            {
                throw new Exception("Room does not exist");
            }
        }

        public List<LobbyRoom> GetRooms()
        {
            return _lobbyRooms;
        }


        public void DeleteRoom(string roomName)
        {
            var room = _lobbyRooms.FirstOrDefault(r => r.RoomName == roomName);
            if (room != null)
            {
                _lobbyRooms.Remove(room);
                Console.WriteLine($"Room deleted: {roomName}");
            }
            else
            {
                Console.WriteLine($"Room does not exist: {roomName}");
            }
        }

        public bool DoesRoomExist(string roomName)
        {
            return _lobbyRooms.Any(r => r.RoomName == roomName);
        }





    }

}

