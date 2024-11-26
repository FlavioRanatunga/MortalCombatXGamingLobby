using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GamingLobbyDataTier
{
    [DataContract]

    public class UserDatabase
    {
        [DataMember]
        public List<User> _loggedInUsers { get; set; }

        public  UserDatabase() {
        _loggedInUsers = new List<User>();
        Console.WriteLine("User Database Initiated");

        }

        public List<User> GetLoggedInUsers()
        {
            return _loggedInUsers;
        }

        public void AddUser(string username)
        {
            User newUser= new User(username);
            _loggedInUsers.Add(newUser);
        }

        public void RemoveUser(string username)
        {
            _loggedInUsers.Remove(_loggedInUsers.Find(user => user.GetUserName().Equals(username)));
        }

        public bool DoesUserNameExist(string username)
        {
           return _loggedInUsers.Any(user=>user.GetUserName().Equals(username));
        }

        public User GetByUsername(string username)
        {
            return _loggedInUsers.Find(user => user.GetUserName().Equals(username));
        }

        public User GetUserByUsername(string username)
        {
            return _loggedInUsers.Find(user => user.GetUserName().Equals(username));
        }

    }
}
