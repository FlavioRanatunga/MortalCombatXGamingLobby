using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingLobbyDataTier
{
    public class PMDatabase
    {
        public Dictionary<(string, string), List<Message>> pMessages;

        public PMDatabase()
        {
            pMessages = new Dictionary<(string, string), List<Message>>();
        }

        public void AddMessage(string senderUsername, string receiverUsername, string message)
        {
            var pmKey = (senderUsername, receiverUsername);
            var pmReverseKey = (receiverUsername, senderUsername);

            if (!pMessages.ContainsKey(pmKey))
            {
                pMessages[pmKey] = new List<Message>();
            }

            if (!pMessages.ContainsKey(pmReverseKey))
            {
                pMessages[pmReverseKey] = new List<Message>();
            }

            // Determine the next available MessageId
            int messageId = Math.Max(pMessages[pmKey].Count, pMessages[pmReverseKey].Count);

            var newMessage = new Message(message, senderUsername, messageId);

            // Add message to both conversation keys
            pMessages[pmKey].Add(newMessage);
            pMessages[pmReverseKey].Add(newMessage);
        }

        public List<Message> GetMessages(string senderUsername, string receiverUsername)
        {
            var pmKey = (senderUsername, receiverUsername);

            if (pMessages.ContainsKey(pmKey))
            {
                return pMessages[pmKey];
            }
            return new List<Message>();
        }

    }
}
