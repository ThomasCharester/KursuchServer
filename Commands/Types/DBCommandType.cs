namespace KursuchServer;

public enum DBCommandType
{
    AccountGetPassword = 0,
    AccountGetAll = 1,
    AccountGetAllAsString = 2,
    AccountAdd = 3,
    AccountDelete = 4,
    CheckAdminKey = 5,
    CheckAccountData = 6,
}