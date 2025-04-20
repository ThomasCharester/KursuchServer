using Npgsql;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace KursuchServer
{
    class ServerApp
    {
        private const int Port = 8888;
        private static TcpListener _listener;
        static void Main(string[] args)
        {
            
            try
            {
                // Создаем слушателя на указанном порту
                _listener = new TcpListener(IPAddress.Any, Port);
                _listener.Start();
                Console.WriteLine($"Сервер запущен на порту {Port} в {IPAddress.Any}. Ожидание подключений...");

                while (true)
                {
                    // Принимаем новое подключение
                    TcpClient client = _listener.AcceptTcpClient();
                    Console.WriteLine($"Подключен клиент: {client.Client.RemoteEndPoint}");

                    // Создаем поток для обработки клиента
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                    clientThread.Start(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                if (_listener != null)
                    _listener.Stop();
            }
            
        }
        
        private static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
        
            try
            {
                // Получаем сетевой поток для чтения и записи
                NetworkStream stream = client.GetStream();

                byte[] buffer = new byte[1024];
                int bytesRead;

                // Читаем данные от клиента
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Получено от клиента: {data}");

                    // Отправляем ответ
                    string response = $"Сервер получил ваше сообщение: {data}";
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseData, 0, responseData.Length);
                    Console.WriteLine($"Отправлено клиенту: {response}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке клиента: {ex.Message}");
            }
            finally
            {
                // Закрываем соединение
                client.Close();
                Console.WriteLine("Клиент отключен");
            }
        }
        
    }
}