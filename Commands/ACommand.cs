using System.Net.Sockets;
using KursuchServer.Services;

namespace KursuchServer;

public class ACommand : Command
{
    public ACommandType SubType;

    public ACommand(TcpClient tcpClient, String query, ACommandType subType, Action<Object> outputFunc = null)
    {
        OutputFunc = outputFunc;
        Type = CommandType.ACommand;
        Client = tcpClient;
        Query = query;
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
                {
                    var invoker = AccountService.Instance.GetClient(Client).Value;

                    var toDelete = Query.StringToAccount();
                    if (invoker.AdminKey == "NAN")
                        throw new Exception("Читы не разрешены");
                    if (invoker.Login == toDelete.Login)
                        throw new Exception("Себе в ногу не стреляем");
                    if (AccountService.Instance.GetClient(toDelete.Login).HasValue)
                        throw new Exception("Пользователь ещё в сети");
                    
                    Query = toDelete.Login;
                    AccountService.Instance.RequestDelete(this);
                }
                    break;
                case ACommandType.AccountLogout:
                    AccountService.Instance.Logout(this);
                    break;
                case ACommandType.AccountModify:
                {
                    var invoker = AccountService.Instance.GetClient(Client).Value;
                    if (invoker.AdminKey == "NAN")
                        throw new Exception("Читы не разрешены");
                    AccountService.Instance.RequestModify(this);
                }
                    break;
                case ACommandType.AccountModifySelf:
                {
                    var invoker = AccountService.Instance.GetClient(Client).Value;
                    AccountService.Instance.RequestModifySelf(this);
                }
                    break;
            }
        }
        catch (Exception ex)
        {
            ServerApp.Instance.AddCommand(new TCPCommand(Client, $"ew{DataParsingExtension.QuerySplitter}{ex.Message}",
                TCPCommandType.SendSingleValue));
        }
    }

    public override void Undo()
    {
    }
}