using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GamingLobbyDataTier
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public string MessageText { get; set; }

        [DataMember]
        public string SenderUserName { get; set; }

        [DataMember]
        private string RecieverUserName { get; set; }

        [DataMember]
        public DateTime MessageTimeStamp { get; set; }

        [DataMember]
        public int MessageId { get; set; }

        [DataMember]
        public bool IsFile {  get; set; }

        public Message(string messageText, string senderUserName, int messageId)
        {
            MessageText = messageText;
            SenderUserName = senderUserName;
            MessageTimeStamp = DateTime.Now;
            MessageId = messageId;
            Console.WriteLine(MessageTimeStamp);  
            Debug.WriteLine(MessageTimeStamp.ToString());
            Console.WriteLine("Message initiated");

        }

        public Message(string messageText, string senderUserName, int messageId,bool isFile)
        {
            MessageText = messageText;
            SenderUserName = senderUserName;
            MessageTimeStamp = DateTime.Now;
            MessageId = messageId;
            IsFile = isFile;
        }

        public override string ToString()
        {
            return $"{SenderUserName}: {MessageText}";
        }


    }
}
