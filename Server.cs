using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
namespace KursuchServer;

public class Server
{
    String connectionString = "Host=localhost;Username=postgres;Password=E1!$;Database=postgres";
    public Server()
    {
        //StartServer();
        GetDrugData("Mephedrone");

    }
    async void StartServer()
    {
        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        
// Insert some data
        await using (var cmd = dataSource.CreateCommand("INSERT INTO data (some_field) VALUES ($1)"))
        {
            cmd.Parameters.AddWithValue("Hello world");
            await cmd.ExecuteNonQueryAsync();
        }

// Retrieve all rows
        await using (var cmd = dataSource.CreateCommand("SELECT some_field FROM data"))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine(reader.GetString(0));
            }
        }
    }

    async Task<String> GetSomeData(NpgsqlDataSource dataSource)
    {
        StringBuilder str = new();
        await using (var cmd = dataSource.CreateCommand("SELECT some_field FROM data"))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine(reader.GetString(0));
                str.Append(reader.GetString(0));
            }
        }
        return str.ToString();
    }
    
    async void SetDrugData( String data)
    {
        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        await using (var cmd = dataSource.CreateCommand("INSERT INTO DRUGS (Name,Concentration) VALUES ($1,$2)"))
        {
            cmd.Parameters.AddWithValue(data);
            cmd.Parameters.AddWithValue(52);
            await cmd.ExecuteNonQueryAsync();
        }
        Console.WriteLine("INSERTION COMPLETE");
    }
    
    async Task<String> GetDrugData(String drugName)
    {
        Console.WriteLine($"GATHERING {drugName} DATA");
        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        StringBuilder str = new();
        await using (var cmd = dataSource.CreateCommand("SELECT * FROM DRUGS WHERE Name = ($1)"))
        {
            cmd.Parameters.AddWithValue(drugName);
            
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine(reader.GetString(0) + reader.GetInt32(1));
                    str.Append(reader.GetString(0));
                    str.Append(reader.GetInt32(1));
                }
            }
        }
        Console.WriteLine("GATHERING COMPLETE");
        return str.ToString();
    }
    
    async void CreateDrugTable()
    {
        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        await using (var cmd = dataSource.CreateCommand("CREATE TABLE DRUGS(\nName VARCHAR(255) not null primary key,\nConcentration INT\n);\n"))
        {
            await cmd.ExecuteNonQueryAsync();
        }
        Console.WriteLine("CREATION COMPLETE");
    }

    async void CreateDiseaseTable()
    {
        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        await using (var cmd = dataSource.CreateCommand("CREATE TABLE DRUGS(\nName VARCHAR(255) not null primary key,\nConcentration INT\n);\n"))
        {
            await cmd.ExecuteNonQueryAsync();
        }
        Console.WriteLine("CREATION COMPLETE");
    }
}