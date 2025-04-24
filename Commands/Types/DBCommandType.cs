namespace KursuchServer;

public enum DBCommandType
{
    AccountGetPassword = 0,
    AccountGetAll = 1,
    AccountGetAllAsString = 2,
    AccountAdd = 3,
    AccountDelete = 4,
    AccountModify = 5,
    CheckAdminKey = 6,
    CheckAccountData = 7,
}