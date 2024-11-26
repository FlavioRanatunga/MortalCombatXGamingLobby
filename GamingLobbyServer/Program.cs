using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using IGamingLobbyServer;

namespace GamingLobbyServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //This should *definitely* be more descriptive.
            Console.WriteLine("Welcome To Mortal Combat X Gaming Lobby Server");

            //This is the actual host service system
            ServiceHost host;

            //This represents a tcp/ip binding in the Windows network stack
            NetTcpBinding tcp = new NetTcpBinding();

            //Bind server to the implementation of Gaming Lobby Server
            host = new ServiceHost(typeof(GamingLobbyServerImpl));

            //Present the publicly accessible interface to the client. 0.0.0.0 tells .net to
            // accept on any interface. :8100 means this will use port 8100.

            host.AddServiceEndpoint(typeof(GamingLobbyServerInterface), tcp,
           "net.tcp://0.0.0.0:8100/MortalCombatXLobbyServer");

            //And open the host for business!        
            host.Open();

            Console.WriteLine("System Online");
            Console.ReadLine();

            //Don't forget to close the host after you're done!

            host.Close();
        }
    }
}
