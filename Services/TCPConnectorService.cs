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

                    if (AccountService.Instance.GetClient(tcpClient) == null)
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
            if (client != null)
                ServerApp.Instance.AddCommand(new ACommand(tcpClient,
                    client.Login + ',' +
                    client.Password,
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
                        ServerApp.Instance.AddCommand(new ACommand(tcpClient,
                            request.Split(DataParsingExtension.QuerySplitter)[1],
                            ACommandType.GetAllAccounts));
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
                    case 'a':
                        ServerApp.Instance.AddCommand(new ACommand(tcpClient,
                            request.Split(DataParsingExtension.QuerySplitter)[1],
                            ACommandType.AccountAdd));
                        break;
                    case 'k':
                        switch (request[2])
                        {
                            case 'l':
                                ServerApp.Instance.AddCommand(new ACommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    ACommandType.GetAllAdminKeys));
                                break;
                            case 'a':
                                ServerApp.Instance.AddCommand(new ACommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    ACommandType.AdminKeyAdd));
                                break;
                            case 'd':
                                ServerApp.Instance.AddCommand(new ACommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    ACommandType.AdminKeyDelete));
                                break;
                            case 'm':
                                ServerApp.Instance.AddCommand(new ACommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    ACommandType.AdminKeyModify));
                                break;
                        }

                        break;

                }

                break;
            case 'g':
            {
                switch (request[1])
                {
                    // case 'a':
                    //     ServerApp.Instance.AddCommand(new DBCommand(tcpClient, request.Split(DataParsingExtension.QuerySplitter)[1],
                    //         DBCommandType.AccountAdd));
                    //     break;
                    case 'g': // game
                        switch (request[2])
                        {
                            case 'g':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.GameGet));
                                break;
                            case 'l':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.GameGetAll));
                                break;
                            case 'a':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.GameAdd));
                                break;
                            case 'd':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.GameDelete));
                                break;
                            case 'm':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.GameModify));
                                break;
                        }

                        break;

                    case 'p': // payment method
                        switch (request[2])
                        {
                            case 'g':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.PaymentMethodGet));
                                break;
                            case 'l':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.PaymentMethodGetAll));
                                break;
                            case 'a':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.PaymentMethodAdd));
                                break;
                            case 'd':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.PaymentMethodDelete));
                                break;
                            case 'm':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.PaymentMethodModify));
                                break;
                        }

                        break;
                    case 's': // seller
                        switch (request[2])
                        {
                            case 'g':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.SellerGet));
                                break;
                            case 'l':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.SellerGetAll));
                                break;
                            case 'a':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.SellerAdd));
                                break;
                            case 'd':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.SellerDelete));
                                break;
                            case 'm':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.SellerModify));
                                break;
                        }

                        break;
                    case 't': // good
                        switch (request[2])
                        {
                            case 'g':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.GoodGet));
                                break;
                            case 'l':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.GoodGetAll));
                                break;
                            case 'a':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.GoodAdd));
                                break;
                            case 'd':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.GoodDelete));
                                break;
                            case 'm':
                                ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                    request.Split(DataParsingExtension.QuerySplitter)[1],
                                    GCommandType.GoodModify));
                                break;
                            case 'p':
                                switch (request[3])
                                {
                                    case 'l':
                                        ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                            request.Split(DataParsingExtension.QuerySplitter)[1],
                                            GCommandType.GoodGetAllAP));
                                        break;
                                    case 'd':
                                        ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                            request.Split(DataParsingExtension.QuerySplitter)[1],
                                            GCommandType.GoodDeleteAP));
                                        break;
                                }
                                break;
                            case 's':
                                switch (request[3])
                                {
                                    case 'l':
                                        ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                            request.Split(DataParsingExtension.QuerySplitter)[1],
                                            GCommandType.GoodGetAllSeller));
                                        break;
                                    case 'd':
                                        ServerApp.Instance.AddCommand(new GCommand(tcpClient,
                                            request.Split(DataParsingExtension.QuerySplitter)[1],
                                            GCommandType.GoodDeleteSeller));
                                        break;
                                }
                                break;
                        }

                        break;
                }
            }
                break;
            // case 'p': // Обработка команд растений
            // {
            //     switch (request[1])
            //     {
            //         case 'd': // Работа с болезнями (Diseases)
            //             switch (request[2])
            //             {
            //                 case 'a': // Добавление
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.DiseaseAdd, SendSingleValue));
            //                     break;
            //                 case 'd': // Удаление
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.DiseaseDelete, SendSingleValue));
            //                     break;
            //                 case 'm': // Модификация
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.DiseaseModify, SendSingleValue));
            //                     break;
            //                 case 'g': // Получение
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.DiseaseGet, SendSingleValue));
            //                     break;
            //                 case 'l': // Все записи
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.DiseaseGetAll, SendMultipleValue));
            //                     break;
            //             }
            //
            //             break;
            //
            //         case 'm': // Работа с лекарствами (Medicines)
            //             switch (request[2])
            //             {
            //                 case 'a':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.MedicineAdd, SendSingleValue));
            //                     break;
            //                 case 'd':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.MedicineDelete, SendSingleValue));
            //                     break;
            //                 case 'm':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.MedicineModify, SendSingleValue));
            //                     break;
            //                 case 'g':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.MedicineGet, SendSingleValue));
            //                     break;
            //                 case 'l':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.MedicineGetAll, SendMultipleValue));
            //                     break;
            //             }
            //
            //             break;
            //
            //         case 'p': // Работа с растениями (Plants)
            //             switch (request[2])
            //             {
            //                 case 'a':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.PlantAdd, SendSingleValue));
            //                     break;
            //                 case 'd':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.PlantDelete, SendSingleValue));
            //                     break;
            //                 case 'm':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.PlantModify, SendSingleValue));
            //                     break;
            //                 case 'g':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.PlantGet, SendSingleValue));
            //                     break;
            //                 case 'l':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.PlantGetAll, SendMultipleValue));
            //                     break;
            //             }
            //
            //             break;
            //
            //         case 'x': // Связи растения-лекарства (PlantMedicines)
            //             switch (request[2])
            //             {
            //                 case 'a':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.PlantMedicineAdd, SendSingleValue));
            //                     break;
            //                 case 'd':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.PlantMedicineDelete, SendSingleValue));
            //                     break;
            //                 case 'l':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.PlantMedicineGetAll, SendMultipleValue));
            //                     break;
            //             }
            //
            //             break;
            //
            //         case 'y': // Связи лекарства-болезни (MedicineDiseases)
            //             switch (request[2])
            //             {
            //                 case 'a':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.MedicineDiseaseAdd, SendSingleValue));
            //                     break;
            //                 case 'd':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.MedicineDiseaseDelete, SendSingleValue));
            //                     break;
            //                 case 'l':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.MedicineDiseaseGetAll, SendMultipleValue));
            //                     break;
            //             }
            //
            //             break;
            //
            //         case 'z': // Дозировки (Dosages)
            //             switch (request[2])
            //             {
            //                 case 'a':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.DosageAdd, SendSingleValue));
            //                     break;
            //                 case 'd':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.DosageDelete, SendSingleValue));
            //                     break;
            //                 case 'g':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.DosageGet, SendSingleValue));
            //                     break;
            //                 case 'l':
            //                     ServerApp.Instance.AddCommand(new PCommand(tcpClient,
            //                         request.Split(DataParsingExtension.QuerySplitter)[1],
            //                         PCommandType.DosageGetAll, SendMultipleValue));
            //                     break;
            //             }
            //
            //             break;
            //     }
            // }
            //     break;
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

    public void SendSingleValueLabeled(Object dataObj) //
    {
        Command data = (Command)dataObj;

        NetworkStream stream = data.Client.GetStream();

        // Отправляем ответ
        byte[] responseData = Encoding.UTF8.GetBytes(data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                                                     DataParsingExtension.QuerySplitter + (String)data.Output + '\n'); // 
        stream.Write(responseData, 0, responseData.Length);
    }

    public void SendMultipleValue(Object dataObj) //
    {
        Command data = (Command)dataObj;

        NetworkStream stream = data.Client.GetStream();

        StringBuilder builder = new();
        builder.Append(data.Query.Split(DataParsingExtension.QuerySplitter)[0] + DataParsingExtension.QuerySplitter);

        var values = (List<String>)data.Output;
        if(values.Count > 0)
        {
            foreach (var value in values)
                builder.Append(value + DataParsingExtension.AdditionalQuerySplitter);
            builder.Remove(builder.Length - 1, 1);
        }
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
    
    public void GenericGetAllResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Output == null) result.Query = "ERR";
        
        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"es{DataParsingExtension.QuerySplitter}TF{DataParsingExtension.QuerySplitter}Ошибка",
                TCPCommandType.SendSingleValue));
            return;
        }
        
        ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
            result,
            TCPCommandType.SendMultipleValueLabeled));
        //TCPConnectorService.Instance.SendMultipleValue(result);
    }
    public void GenericResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"es{DataParsingExtension.QuerySplitter}TF{DataParsingExtension.QuerySplitter}Ошибка",
                TCPCommandType.SendSingleValue));
            return;
        }
// надо ли кидать добавленные данные назад?
        ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
            result.Query.Split(DataParsingExtension.QuerySplitter)[0] + DataParsingExtension.QuerySplitter,
            TCPCommandType.SendSingleValue));
    }
}