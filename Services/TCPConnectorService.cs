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
                                ServerApp.Instance.AddCommand(new ACommand(tcpClient,
                                    data.Split(DataParsingExtension.QuerySplitter)[1],
                                    ACommandType.AccountLogin));
                                break;
                            case 'r':
                                ServerApp.Instance.AddCommand(new ACommand(tcpClient,
                                    data.Split(DataParsingExtension.QuerySplitter)[1],
                                    ACommandType.AccountRegister));
                                break;
                        }
                    }

                    if (!AccountService.Instance.GetClient(tcpClient).HasValue)
                        continue;

                    loginAttempt = true;

                    UserRequestProcess(data, tcpClient);
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
            if (tcpClient.Connected) Console.WriteLine($"Клиент {tcpClient.Client.RemoteEndPoint} отключен");
            
            var client = AccountService.Instance.GetClient(tcpClient);
            if (client.HasValue)
                ServerApp.Instance.AddCommand(new ACommand(tcpClient,
                    client.Value.Login + ',' +
                    client.Value.Password,
                    ACommandType.AccountLogout));
            
            tcpClient.Close();
        }
    }

    private void UserRequestProcess(String request, in TcpClient tcpClient)
    {
        switch (request[0])
        {
            case 'a':
                switch (request[1])
                {
                    // case 'a':
                    //     ServerApp.Instance.AddCommand(new DBCommand(tcpClient, request.Split(DataParsingExtension.QuerySplitter)[1],
                    //         DBCommandType.AccountAdd));
                    //     break;
                    case 'l':
                        ServerApp.Instance.AddCommand(new DBCommand(tcpClient,
                            request.Split(DataParsingExtension.QuerySplitter)[1],
                            DBCommandType.ValueGetAll, SendMultipleValue));
                        break;
                    case 'd':
                        ServerApp.Instance.AddCommand(new ACommand(tcpClient,
                            request.Split(DataParsingExtension.QuerySplitter)[1],
                            ACommandType.AccountDelete));
                        break;
                    case 'o':
                        ServerApp.Instance.AddCommand(new ACommand(tcpClient,
                            request.Split(DataParsingExtension.QuerySplitter)[1],
                            ACommandType.AccountLogout));
                        break;
                    case 's':
                        ServerApp.Instance.AddCommand(new ACommand(tcpClient,
                            request.Split(DataParsingExtension.QuerySplitter)[1],
                            ACommandType.AccountModifySelf));
                        break;
                    case 'm':
                        ServerApp.Instance.AddCommand(new ACommand(tcpClient,
                            request.Split(DataParsingExtension.QuerySplitter)[1],
                            ACommandType.AccountModify));
                        break;
                }

                break;
            // case 'g':
            // {
            //     switch (request[1])
            //     {
            //         // case 'a':
            //         //     ServerApp.Instance.AddCommand(new DBCommand(tcpClient, request.Split(DataParsingExtension.QuerySplitter)[1],
            //         //         DBCommandType.AccountAdd));
            //         //     break;
            //         case 'g': // game
            //             switch (request[2])
            //             {
            //                 case 'g':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.GameGet, SendMultipleValue));
            //                     break;
            //                 case 'l':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.GameGetAll, SendMultipleValue));
            //                     break;
            //                 case 'a':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.GameAdd, SendMultipleValue));
            //                     break;
            //                 case 'd':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.GameDelete, SendMultipleValue));
            //                     break;
            //                 case 'm':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.GameModify, SendMultipleValue));
            //                     break;
            //             }
            //
            //             break;
            //         case 'a': // aquire type
            //             switch (request[2])
            //             {
            //                 case 'g':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.AquireTypeGet, SendMultipleValue));
            //                     break;
            //                 case 'l':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.AquireTypeGetAll, SendMultipleValue));
            //                     break;
            //                 case 'a':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.AquireTypeAdd, SendMultipleValue));
            //                     break;
            //                 case 'd':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.AquireTypeDelete, SendMultipleValue));
            //                     break;
            //                 case 'm':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.AquireTypeModify, SendMultipleValue));
            //                     break;
            //             }
            //
            //             break;
            //         case 'p': // payment method
            //             switch (request[2])
            //             {
            //                 case 'g':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.PaymentMethodGet, SendMultipleValue));
            //                     break;
            //                 case 'l':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.PaymentMethodGetAll, SendMultipleValue));
            //                     break;
            //                 case 'a':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.PaymentMethodAdd, SendMultipleValue));
            //                     break;
            //                 case 'd':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.PaymentMethodDelete, SendMultipleValue));
            //                     break;
            //                 case 'm':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.PaymentMethodModify, SendMultipleValue));
            //                     break;
            //             }
            //
            //             break;
            //         case 's': // seller
            //             switch (request[2])
            //             {
            //                 case 'g':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.SellerGet, SendMultipleValue));
            //                     break;
            //                 case 'l':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.SellerGetAll, SendMultipleValue));
            //                     break;
            //                 case 'a':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.SellerAdd, SendMultipleValue));
            //                     break;
            //                 case 'd':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.SellerDelete, SendMultipleValue));
            //                     break;
            //                 case 'm':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.SellerModify, SendMultipleValue));
            //                     break;
            //             }
            //
            //             break;
            //         case 't': // good
            //             switch (request[2])
            //             {
            //                 case 'g':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.GoodGet, SendMultipleValue));
            //                     break;
            //                 case 'l':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.GoodGetAll, SendMultipleValue));
            //                     break;
            //                 case 'a':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.GoodAdd, SendMultipleValue));
            //                     break;
            //                 case 'd':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.GoodDelete, SendMultipleValue));
            //                     break;
            //                 case 'm':
            //                     ServerApp.Instance.AddCommand(new GCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         GCommandType.GoodModify, SendMultipleValue));
            //                     break;
            //             }
            //
            //             break;
            //     }
            // }
            //     break;
            case 'p': // Обработка команд растений
            {
                switch (request[1])
                {
                    case 'd': // Работа с болезнями (Diseases)
                        switch (request[2])
                        {
                            case 'a': // Добавление
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.DiseaseAdd, SendMultipleValue));
                                break;
                            case 'd': // Удаление
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.DiseaseDelete, SendMultipleValue));
                                break;
                            case 'm': // Модификация
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.DiseaseModify, SendMultipleValue));
                                break;
                            case 'g': // Получение
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.DiseaseGet, SendMultipleValue));
                                break;
                            case 'l': // Все записи
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.DiseaseGetAll, SendMultipleValue));
                                break;
                        }

                        break;

                    case 'm': // Работа с лекарствами (Medicines)
                        switch (request[2])
                        {
                            case 'a':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.MedicineAdd, SendMultipleValue));
                                break;
                            case 'd':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.MedicineDelete, SendMultipleValue));
                                break;
                            case 'm':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.MedicineModify, SendMultipleValue));
                                break;
                            case 'g':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.MedicineGet, SendMultipleValue));
                                break;
                            case 'l':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.MedicineGetAll, SendMultipleValue));
                                break;
                        }

                        break;

                    case 'p': // Работа с растениями (Plants)
                        switch (request[2])
                        {
                            case 'a':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.PlantAdd, SendMultipleValue));
                                break;
                            case 'd':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.PlantDelete, SendMultipleValue));
                                break;
                            case 'm':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.PlantModify, SendMultipleValue));
                                break;
                            case 'g':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.PlantGet, SendMultipleValue));
                                break;
                            case 'l':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.PlantGetAll, SendMultipleValue));
                                break;
                        }

                        break;

                    case 'x': // Связи растения-лекарства (PlantMedicines)
                        switch (request[2])
                        {
                            case 'a':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.PlantMedicineAdd, SendMultipleValue));
                                break;
                            case 'd':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.PlantMedicineDelete, SendMultipleValue));
                                break;
                            case 'l':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.PlantMedicineGetAll, SendMultipleValue));
                                break;
                        }

                        break;

                    case 'y': // Связи лекарства-болезни (MedicineDiseases)
                        switch (request[2])
                        {
                            case 'a':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.MedicineDiseaseAdd, SendMultipleValue));
                                break;
                            case 'd':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.MedicineDiseaseDelete, SendMultipleValue));
                                break;
                            case 'l':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.MedicineDiseaseGetAll, SendMultipleValue));
                                break;
                        }

                        break;

                    case 'z': // Дозировки (Dosages)
                        switch (request[2])
                        {
                            case 'a':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.DosageAdd, SendMultipleValue));
                                break;
                            case 'd':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.DosageDelete, SendMultipleValue));
                                break;
                            case 'g':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.DosageGet, SendMultipleValue));
                                break;
                            case 'l':
                                ServerApp.Instance.AddCommand(new PCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    PCommandType.DosageGetAll, SendMultipleValue));
                                break;
                        }

                        break;
                }
            }
                break;
        }
    }

    public void SendSingleValue(TCPCommand data) //
    {
        NetworkStream stream = data.Client.GetStream();

        // Отправляем ответ
        byte[] responseData = Encoding.UTF8.GetBytes(data.Query + '\n');
        stream.Write(responseData, 0, responseData.Length);
    }

    public void SendSingleValue(Object dataObj) //
    {
        Command data = (Command)dataObj;

        NetworkStream stream = data.Client.GetStream();

        // Отправляем ответ
        byte[] responseData = Encoding.UTF8.GetBytes((String)data.Output + '\n');
        stream.Write(responseData, 0, responseData.Length);
    }

    public void SendMultipleValue(Object dataObj) //
    {
        Command data = (Command)dataObj;

        NetworkStream stream = data.Client.GetStream();

        StringBuilder builder = new();

        foreach (var value in (List<String>)data.Output)
            builder.Append(value + '\n');

        // Отправляем ответ
        byte[] responseData = Encoding.UTF8.GetBytes(builder.ToString());
        stream.Write(responseData, 0, responseData.Length);
    }

    public void KillClient(TCPCommand data) //
    {
        NetworkStream stream = data.Client.GetStream();

        // Отправляем ответ
        byte[] responseData = Encoding.UTF8.GetBytes(data.Query + " is last words \n");
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