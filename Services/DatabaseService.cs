using System.Linq.Expressions;
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

    public async void AddAccount(Account account) //
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
        }
        catch
        {
            Console.WriteLine("Error when adding account");
        }
    }

    public async void DeleteAccount(String login) //
    {
        await using var dataSource = NpgsqlDataSource.Create(_connectionString);

        await using (var cmd = dataSource.CreateCommand("DELETE FROM Accounts WHERE login = $1;"))
        {
            cmd.Parameters[0].Value = login;
            await cmd.ExecuteNonQueryAsync();
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
}