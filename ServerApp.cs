using Npgsql;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using KursuchServer.Services;


namespace KursuchServer
{
    class ServerApp
    {
        private Queue<Command> _commands = new();
        private Thread _commandExecutionThread;
        private static ServerApp instance = null;
        public static ServerApp Instance
        {
            get
            {
                if (instance == null) instance = new ServerApp();
                return instance;
            }
            private set { instance = value; }
        }

        private ServerApp()
        {
            InitializeServices();
        }
        static void Main(string[] args)
        {
            ServerApp serverApp = new();
            
            Account test  = new Account("TChar","*****","qwertyuiop");
            //DatabaseService.Instance.AddAccount("qwertyuiop", test);
            // foreach (var acc in DatabaseService.Instance.GetAllAccounts().Result)
            // {
            //     Console.WriteLine(acc);
            // }
            DatabaseService.Instance.GetAllAccounts();
            
            Console.ReadKey();
        }

        private void CommandTaskExecution()
        {
            
        }

        public void AddCommand(Command command)
        {
            _commands.Enqueue(command);
        }

        private void InitializeServices()
        {
            StaticDataService.Instance.LoadServicesConfigs();
        }
    }
}