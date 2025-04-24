using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KursuchServer.Services;

public class TCPConnectorService
{
    private String _ipAddressString;
    private String _port;
    private TcpListener _tcpListener;
    private bool _serverRunning = false;

    private static TCPConnectorService instance = null;

    public static TCPConnectorService Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    public TCPConnectorService(String ipAddressString, String port)
    {
        instance = this;

        _ipAddressString = ipAddressString;
        _port = port;
    }

    public void StartServer() //
    {
        _serverRunning = true;
        try
        {
            // Запускаем сервер
            _tcpListener = new TcpListener(IPAddress.Any, int.Parse(_port)); //Parse(_ipAddressString) int.Parse(_port)
            _tcpListener.Start();
            Console.WriteLine($"Сервер запущен на порту {_port}. Ожидание подключений...");

            // Основной цикл обработки подключений
            while (_serverRunning)
            {
                if (_tcpListener.Pending())
                {
                    TcpClient client = _tcpListener.AcceptTcpClient();

                    // lock (_connectedClients)
                    // {
                    //     _connectedClients.Add(client);
                    // }
                    Console.WriteLine($"Подключен клиент: {client.Client.RemoteEndPoint}");

                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.Start();
                }
                else
                {
                    Thread.Sleep(100); // Небольшая пауза, чтобы снизить нагрузку на CPU
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        finally
        {
            StopServer();
        }
    }

    public void StopServer() //
    {
        _serverRunning = false;

        // Останавливаем прослушивание
        _tcpListener?.Stop();

        // Закрываем все клиентские подключения
        // lock (_connectedClients)
        // {
        //     foreach (var client in _connectedClients)
        //     {
        //         client.Close();
        //     }
        //     _connectedClients.Clear();
        // }

        Console.WriteLine("Сервер остановлен");
    }

    private void HandleClient(TcpClient tcpClient) //
    {
        try
        {
            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;
            bool loginAttempt = false;

            while (IsClientConnected(tcpClient))
            {
                if (stream.DataAvailable)
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"{tcpClient.Client.RemoteEndPoint}:{data}");
                    if (!loginAttempt)
                    {
                        switch (data[0])
                        {
                            case 'l':
                                ServerApp.Instance.AddCommand(new ACommand(tcpClient, data.Split(';')[1],
                                    ACommandType.AccountLogin));
                                break;
                            case 'r':
                                ServerApp.Instance.AddCommand(new ACommand(tcpClient, data.Split(';')[1],
                                    ACommandType.AccountRegister));
                                break;
                        }
                    }

                    if (!AccountService.Instance.GetClient(tcpClient).HasValue)
                        continue;

                    loginAttempt = true;

                    switch (data[0])
                    {
                        case 'a':
                            switch (data[1])
                            {
                                // case 'a':
                                //     ServerApp.Instance.AddCommand(new DBCommand(tcpClient, data.Split(';')[1],
                                //         DBCommandType.AccountAdd));
                                //     break;
                                case 'g':
                                    ServerApp.Instance.AddCommand(new DBCommand(tcpClient, data.Split(';')[1],
                                        DBCommandType.AccountGetAllAsString, SendToClient));
                                    break;
                                case 'd':
                                    ServerApp.Instance.AddCommand(new ACommand(tcpClient, data.Split(';')[1],
                                        ACommandType.AccountDelete));
                                    break;
                                case 'o':
                                    ServerApp.Instance.AddCommand(new ACommand(tcpClient, data.Split(';')[1],
                                        ACommandType.AccountLogout));
                                    break;
                                case 'm':
                                    ServerApp.Instance.AddCommand(new ACommand(tcpClient, data.Split(';')[1],
                                        ACommandType.AccountModify));
                                    break;
                            }

                            break;
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка с клиентом {tcpClient.Client.RemoteEndPoint}: {ex.Message}");
        }
        finally
        {
            // lock (_connectedClients)
            // {
            //     _connectedClients.Remove(tcpClient);
            // }
            if(tcpClient.Connected) Console.WriteLine($"Клиент {tcpClient.Client.RemoteEndPoint} отключен");
            tcpClient.Close();
        }
    }

    public void SendToClient(TCPCommand data) //
    {
        NetworkStream stream = data.Client.GetStream();

        // Отправляем ответ
        byte[] responseData = Encoding.UTF8.GetBytes(data.Data + '\n');
        stream.Write(responseData, 0, responseData.Length);
    }
    public void SendToClient(Object dataObj) //
    {
        Command data = (Command)dataObj;
        
        NetworkStream stream = data.Client.GetStream();

        // Отправляем ответ
        byte[] responseData = Encoding.UTF8.GetBytes(data.Data + '\n');
        stream.Write(responseData, 0, responseData.Length);
    }
    public void KillClient(TCPCommand data) //
    {
        NetworkStream stream = data.Client.GetStream();

        // Отправляем ответ
        byte[] responseData = Encoding.UTF8.GetBytes(data.Data + " is last words \n");
        stream.Write(responseData, 0, responseData.Length);
        Console.WriteLine($"Клиент {data.Client.Client.RemoteEndPoint} отключен");
        
        data.Client.Close();
    }
    private bool IsClientConnected(TcpClient client)
    {
        if (!client.Connected) return false;

        // Точная проверка состояния соединения
        try
        {
            if (client.Client.Poll(0, SelectMode.SelectRead))
            {
                byte[] dummy = new byte[1];
                return client.Client.Receive(dummy, SocketFlags.Peek) != 0;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

}