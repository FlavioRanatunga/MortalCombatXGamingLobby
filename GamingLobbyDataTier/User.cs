using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GamingLobbyDataTier
{
    [DataContract]

    public class User
    {
        [DataMember]
        public string username { get; set; }

        public User(string username) {
            this.username = username;
        }

        public string GetUserName()
        {
            return username;
        }

        public void SetUserName(string username)
        {
            this.username = username;
        }

        public override string ToString()
    {
        return username;
    }

    }
}
