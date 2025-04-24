using System.Net.Sockets;
using KursuchServer.Services;

namespace KursuchServer;

public class ACommand : Command
{
    public ACommandType SubType;

    public ACommand(TcpClient tcpClient, String data, ACommandType subType, Action<Object> outputFunc = null)
    {
        OutputFunc = outputFunc;
        Type = CommandType.ACommand;
        Client = tcpClient;
        Data = data;
        SubType = subType;
    }

    public ACommand()
    {
        Type = CommandType.ACommand;
    }

    public override void Execute()
    {
        try
        {
            switch (SubType)
            {
                case ACommandType.AccountLogin:
                    AccountService.Instance.RequestLogin(this);
                    break;
                case ACommandType.AccountRegister:
                    AccountService.Instance.RequestRegister(this);
                    break;
                case ACommandType.AccountDelete:
                    var invoker = AccountService.Instance.GetClient(Client).Value;
                    if (invoker.AdminKey == "Nan")
                        throw new Exception("Читы не разрешены");
                    if (invoker.Login == Data.StringToAccount().Login)
                        throw new Exception("Себе в ногу не стреляем");

                    AccountService.Instance.RequestDelete(this);
                    break;
                case ACommandType.AccountLogout:
                    AccountService.Instance.Logout(this);
                    break;
            }
        }
        catch (Exception ex)
        {
            ServerApp.Instance.AddCommand(new TCPCommand(Client, $"ew;{ex.Message}",
                TCPCommandType.SendDefaultMessage));
        }
    }

    public override void Undo()
    {
    }
}