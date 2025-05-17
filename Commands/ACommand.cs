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
                case ACommandType.AccountLogout:
                    AccountService.Instance.Logout(this);
                    break;
                case ACommandType.AccountModifySelf:
                    AccountService.Instance.RequestModifySelf(this);
                    break;
            }

            var client = AccountService.Instance.GetClient(Client);
            if (client is not { SV_Cheats: true }) return;

            switch (SubType)
            {
                case ACommandType.AccountAdd:
                    AccountService.Instance.RequestAddAccount(this);
                    break;
                case ACommandType.AdminKeyAdd:
                    AccountService.Instance.RequestAddAdminKey(this);
                    break;
                case ACommandType.AdminKeyModify:
                    AccountService.Instance.RequestAdminKeyModify(this);
                    break;
                case ACommandType.AdminKeyDelete:
                {
                    var invoker = AccountService.Instance.GetClient(Client);

                    if (invoker.AdminKey == Query)
                        throw new Exception("Себе в ногу не стреляем");
                    // TODO Totally Unsecure LOl

                    AccountService.Instance.RequestAdminKeyDelete(this);
                }
                    break;
                case ACommandType.GetAllAdminKeys:
                    AccountService.Instance.RequestGetAllAdminKeys(this);
                    break;
                case ACommandType.GetAllAccounts:
                    AccountService.Instance.RequestGetAllAccounts(this);
                    break;
                case ACommandType.AccountDelete:
                {
                    var invoker = AccountService.Instance.GetClient(Client);

                    var toDelete = Query.StringToAccount();
                    if (invoker.Login == toDelete.Login)
                        throw new Exception("Себе в ногу не стреляем");
                    if (AccountService.Instance.GetClient(toDelete.Login) != null)
                        throw new Exception("Пользователь ещё в сети");

                    Query = toDelete.Login.DBReadable();
                    AccountService.Instance.RequestDelete(this);
                }
                    break;
                case ACommandType.AccountModify:
                    AccountService.Instance.RequestModify(this);
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