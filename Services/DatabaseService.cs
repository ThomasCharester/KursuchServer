using System.Linq.Expressions;
using KursuchServer.DataStructures;
using Npgsql;

namespace KursuchServer.Services;

public class DatabaseService
{
    private String _connectionString;

    private static DatabaseService instance = null;

    public static DatabaseService Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    public DatabaseService(String connectionString)
    {
        instance = this;
        _connectionString = connectionString;
    }

    public async Task<bool> AddAccount(Account account) //
    {
        try
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using (var cmd = dataSource.CreateCommand(
                             "INSERT INTO Accounts (login,password,adminKey) VALUES ($1,$2,$3);"))
            {
                cmd.Parameters.AddWithValue(account.Login);
                cmd.Parameters.AddWithValue(account.Password);
                cmd.Parameters.AddWithValue(account.AdminKey);
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error when adding account " + ex.Message);
            return false;
        }
    }
    public async Task<bool> ModifyAccount(Command data) //
    {
        Account modifications = data.Data.StringToAccount();
        Client accountToModify = AccountService.Instance.GetClient(data.Client).Value;
        try
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using (var cmd = dataSource.CreateCommand(
                             "UPDATE Accounts SET login = $1, password = $2, adminKey = $3 WHERE login = $4;"))
            {
                cmd.Parameters.AddWithValue(modifications.Login);
                cmd.Parameters.AddWithValue(modifications.Password);
                cmd.Parameters.AddWithValue(modifications.AdminKey);
                cmd.Parameters.AddWithValue(accountToModify.Login);
                await cmd.ExecuteNonQueryAsync();
            }
            accountToModify.Login = modifications.Login;
            accountToModify.Password = modifications.Password;
            accountToModify.AdminKey = modifications.AdminKey;
            
            return true;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error when modifying account " + ex.Message);
            return false;
        }
    }

    public async Task<bool> DeleteAccount(String login) //
    {
        await using var dataSource = NpgsqlDataSource.Create(_connectionString);
        try
        {
            // Если удалять несуществующую запись, то ошибки не будет
            await using (var cmd = dataSource.CreateCommand("DELETE FROM Accounts WHERE login = $1;"))
            {
                cmd.Parameters.AddWithValue(login);
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch
        {
            Console.WriteLine("Error when deleting account");
            return false;
        }
    }

    public async Task<List<Account>> GetAllAccounts() //
    {
        List<Account> accounts = new();
        await using var dataSource = NpgsqlDataSource.Create(_connectionString);

        await using (var cmd = dataSource.CreateCommand("SELECT * FROM Accounts"))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                accounts.Add(
                    (reader.GetString(0) + '|' + reader.GetString(1) + '|' + reader.GetString(2))
                    .StringToAccount());
                //Console.WriteLine(accounts.Last().AccountToString());
            }
        }

        return accounts;
    }

    public async Task<Account> GetAccount(String login) //
    {
        return new Account();
    }

    public async Task<bool> CheckAdminKey(string key)
    {
        await using var dataSource = NpgsqlDataSource.Create(_connectionString);

        await using (var cmd = dataSource.CreateCommand("SELECT adminKey FROM Accounts"))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                if(reader.GetString(0) == key) return true;
            }
        }
        return false;
    }
    public async Task<bool> CheckAccountData(Account account)
    {
        await using var dataSource = NpgsqlDataSource.Create(_connectionString);

        await using (var cmd = dataSource.CreateCommand("SELECT login,password FROM Accounts"))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                if(reader.GetString(0) == account.Login)
                    if (reader.GetString(1) == account.Password) return true;
                    else return false;
            }
        }
        return false;
    }
}