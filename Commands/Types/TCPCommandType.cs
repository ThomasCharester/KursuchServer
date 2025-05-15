namespace KursuchServer;

public enum TCPCommandType
{
    AccountSend,
    AccountSendAll,
    SendSingleValue,
    SendSingleValueLabeled,
    SendMultipleValue,
    SendMultipleValueLabeled,
    DisconnectClient
}