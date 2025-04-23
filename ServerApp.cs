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

        static void Main(string[] args)
        {
            InitializeServices();
            
            // Запускаем фоновую задачу
            Instance._commandExecutionThread = new Thread(Instance.CommandTaskExecution);
            Instance._commandExecutionThread.Start();
            
            TCPConnectorService.Instance.StartServer();
            // Account test  = new Account("TChar","*****","qwertyuiop");
            // DatabaseService.Instance.AddAccount("qwertyuiop", test);
            // DatabaseService.Instance.AddAccount("qwertyuiop", test);
            // foreach (var acc in DatabaseService.Instance.GetAllAccounts().Result)
            // {
            //     Console.WriteLine(acc.AccountToString());
            // }
            // DatabaseService.Instance.GetAllAccounts();
            
            // Console.ReadKey();
        }

        private void CommandTaskExecution()
        {
            while (true)
            {
                if(_commands.Count > 0) _commands.Dequeue().Execute();
                else Thread.Sleep(100);
            }
        }

        public void AddCommand(Command command)
        {
            _commands.Enqueue(command);
        }

        private static void InitializeServices()
        {
            StaticDataService.Instance.LoadServicesConfigs();
        }
    }
}