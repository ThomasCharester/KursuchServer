using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using KursuchServer.DataStructures;
using Npgsql;

namespace KursuchServer.Services;

public class DatabaseService
{
    private String _connectionString;

    private static DatabaseService instance = null;

    public static DatabaseService Instance
    {
        get
        {
            if (instance == null) return instance;
            return instance;
        }
        private set { instance = value; }
    }

    public DatabaseService(String connectionString)
    {
        instance = this;
        _connectionString = connectionString;
    }

    // 3 старые данные, 2 таблица, 1 колонки, 0 новые данные
    public async Task<bool> ModifyValueAnyTable(String query)
    {
        try
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            var condition1 =
                BuildSQLSequence(
                    query.Split(DataParsingExtension.QuerySplitter)[2].Split(DataParsingExtension.ValueSplitter),
                    query.Split(DataParsingExtension.QuerySplitter)[1].Split(DataParsingExtension.ValueSplitter));

            var condition2 =
                BuildSQLCondition(
                    query.Split(DataParsingExtension.QuerySplitter)[2].Split(DataParsingExtension.ValueSplitter),
                    query.Split(DataParsingExtension.QuerySplitter)[4].Split(DataParsingExtension.ValueSplitter));

            await using (var cmd = dataSource.CreateCommand(
                             $"UPDATE {query.Split(DataParsingExtension.QuerySplitter)[3]} SET {condition1} WHERE {condition2}"))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error when modifying data " + ex.Message);
            return false;
        }
    }

    public async Task<bool> CheckDataAnyTable(String query)
    {
        await using var dataSource = NpgsqlDataSource.Create(_connectionString);

        var condition =
            BuildSQLCondition(
                query.Split(DataParsingExtension.QuerySplitter)[1].Split(DataParsingExtension.ValueSplitter),
                query.Split(DataParsingExtension.QuerySplitter)[0].Split(DataParsingExtension.ValueSplitter));

        await using (var cmd = dataSource.CreateCommand(
                         $"SELECT {query.Split(DataParsingExtension.QuerySplitter)[1]} FROM {query.Split(DataParsingExtension.QuerySplitter)[2]} WHERE {condition}"))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            return reader.HasRows;
        }
    }

    private static StringBuilder BuildSQLCondition(in string[] columnNames, in string[] dataToCheck)
    {
        StringBuilder condition = new();

        for (int i = 0; i < dataToCheck.Length; i++)
            condition.Append(columnNames[i] + (" = " + dataToCheck[i]).NANIsNULL() + " AND ");

        condition.Remove(condition.Length - 5, 5);
        return condition;
    }


    private static StringBuilder BuildSQLTabledCondition(in string[] columnNames, in string[] dataToCheck, string tableName)
    {
        StringBuilder condition = new();

        for (int i = 0; i < dataToCheck.Length; i++)
            condition.Append(tableName + '.' + columnNames[i] + (" = " + dataToCheck[i]).NANIsNULL() + " AND ");

        condition.Remove(condition.Length - 5, 5);
        return condition;
    }

    private static StringBuilder BuildSQLJoinCondition(in string[] columnNames, string table1, string table2)
    {
        StringBuilder condition = new();

        for (int i = 0; i < columnNames.Length; i++)
            condition.Append(table1 + '.' + columnNames[i] + " = " + table2 + '.' + columnNames[i] + " AND ");

        condition.Remove(condition.Length - 5, 5);
        return condition;
    }

    private static StringBuilder BuildSQLTabledColumns(in string[] columnNames, string table1)
    {
        StringBuilder condition = new();

        for (int i = 0; i < columnNames.Length; i++)
            condition.Append(table1 + '.' + columnNames[i] + " , ");

        condition.Remove(condition.Length - 3, 3);
        return condition;
    }

    private static StringBuilder BuildSQLSequence(in string[] columnNames, in string[] dataToCheck)
    {
        string query;
        StringBuilder condition = new();

        for (int i = 0; i < dataToCheck.Length; i++)
            condition.Append(columnNames[i] + " = " + dataToCheck[i].NANToNULL() + ",");

        condition.Remove(condition.Length - 1, 1);
        return condition;
    }

    public async Task<List<String>> GetRowsOfAnyTable(String query) //
    {
        List<String> items = new();
        StringBuilder builder = new();
        await using var dataSource = NpgsqlDataSource.Create(_connectionString);

        await using (var cmd = dataSource.CreateCommand(
                         $"SELECT {query.Split(DataParsingExtension.QuerySplitter)[1]} FROM {query.Split(DataParsingExtension.QuerySplitter)[2]}"))
        {
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader.GetFieldType(i) == typeof(string))
                        {
                            if (!reader.IsDBNull(i)) builder.Append(reader.GetString(i) + ',');
                            else builder.Append("NAN,");
                        }
                        else if (reader.GetFieldType(i) == typeof(int))
                        {
                            if (!reader.IsDBNull(i)) builder.Append(reader.GetInt32(i).ToString() + ',');
                            else builder.Append("NAN,");
                        }
                        else if (reader.GetFieldType(i) == typeof(decimal))
                        {
                            if (!reader.IsDBNull(i)) builder.Append(reader.GetDecimal(i).ToString() + ',');
                            else builder.Append("NAN,");
                        }
                        else if (reader.GetFieldType(i) == typeof(bool))
                        {
                            if (!reader.IsDBNull(i)) builder.Append(reader.GetBoolean(i).ToString() + ',');
                            else builder.Append("NAN,");
                        }
                    }

                    builder.Remove(builder.Length - 1, 1);

                    items.Add(builder.ToString());
                    builder.Clear();
                }
            }
        }

        return items;
    }

    public async Task<List<String>> GetRowsOfAnyTableCondition(String query) //
    {
        List<String> items = new();
        StringBuilder builder = new();
        await using var dataSource = NpgsqlDataSource.Create(_connectionString);

        var condition =
            BuildSQLCondition(
                query.Split(DataParsingExtension.QuerySplitter)[4].Split(DataParsingExtension.ValueSplitter),
                query.Split(DataParsingExtension.QuerySplitter)[3].Split(DataParsingExtension.ValueSplitter));

        await using (var cmd = dataSource.CreateCommand(
                         $"SELECT {query.Split(DataParsingExtension.QuerySplitter)[1]} FROM {query.Split(DataParsingExtension.QuerySplitter)[2]} WHERE {condition}"))
        {
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader.GetFieldType(i) == typeof(string))
                        {
                            if (!reader.IsDBNull(i)) builder.Append(reader.GetString(i) + ',');
                            else builder.Append("NAN,");
                        }
                        else if (reader.GetFieldType(i) == typeof(int))
                        {
                            if (!reader.IsDBNull(i)) builder.Append(reader.GetInt32(i).ToString() + ',');
                            else builder.Append("NAN,");
                        }
                        else if (reader.GetFieldType(i) == typeof(decimal))
                        {
                            if (!reader.IsDBNull(i)) builder.Append(reader.GetDecimal(i).ToString() + ',');
                            else builder.Append("NAN,");
                        }
                        else if (reader.GetFieldType(i) == typeof(bool))
                        {
                            if (!reader.IsDBNull(i)) builder.Append(reader.GetBoolean(i).ToString() + ',');
                            else builder.Append("NAN,");
                        }
                    }

                    builder.Remove(builder.Length - 1, 1);

                    items.Add(builder.ToString());
                    builder.Clear();
                }
            }
        }

        return items;
    }

    public async Task<List<String>> GetRowsOfAnyTableJoined(String query) //
    {
        List<String> items = new();
        StringBuilder builder = new();
        await using var dataSource = NpgsqlDataSource.Create(_connectionString);

        var columnsTabled =
            BuildSQLTabledColumns(
                query.Split(DataParsingExtension.QuerySplitter)[2].Split(DataParsingExtension.ValueSplitter),
                query.Split(DataParsingExtension.QuerySplitter)[4]);
        var joinCondition =
            BuildSQLJoinCondition(
                query.Split(DataParsingExtension.QuerySplitter)[3].Split(DataParsingExtension.ValueSplitter),
                query.Split(DataParsingExtension.QuerySplitter)[4],
                query.Split(DataParsingExtension.QuerySplitter)[5]);
        var condition =
            BuildSQLTabledCondition(
                query.Split(DataParsingExtension.QuerySplitter)[2].Split(DataParsingExtension.ValueSplitter),
                query.Split(DataParsingExtension.QuerySplitter)[1].Split(DataParsingExtension.ValueSplitter),
                query.Split(DataParsingExtension.QuerySplitter)[4]);

        await using (var cmd = dataSource.CreateCommand(
                         $"SELECT {columnsTabled} FROM {query.Split(DataParsingExtension.QuerySplitter)[4]} INNER JOIN {query.Split(DataParsingExtension.QuerySplitter)[5]} ON {joinCondition} WHERE {condition}"))
        {
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader.GetFieldType(i) == typeof(string))
                        {
                            if (!reader.IsDBNull(i)) builder.Append(reader.GetString(i) + ',');
                            else builder.Append("NAN,");
                        }
                        else if (reader.GetFieldType(i) == typeof(int))
                        {
                            if (!reader.IsDBNull(i)) builder.Append(reader.GetInt32(i).ToString() + ',');
                            else builder.Append("NAN,");
                        }
                        else if (reader.GetFieldType(i) == typeof(decimal))
                        {
                            if (!reader.IsDBNull(i)) builder.Append(reader.GetDecimal(i).ToString() + ',');
                            else builder.Append("NAN,");
                        }
                        else if (reader.GetFieldType(i) == typeof(bool))
                        {
                            if (!reader.IsDBNull(i)) builder.Append(reader.GetBoolean(i).ToString() + ',');
                            else builder.Append("NAN,");
                        }
                    }

                    builder.Remove(builder.Length - 1, 1);

                    items.Add(builder.ToString());
                    builder.Clear();
                }
            }
        }

        return items;
    }

    public async Task<String> GetValueAnyTable(String query) //
    {
        StringBuilder value = new();
        var condition =
            BuildSQLCondition(
                query.Split(DataParsingExtension.QuerySplitter)[1].Split(DataParsingExtension.ValueSplitter),
                query.Split(DataParsingExtension.QuerySplitter)[0].Split(DataParsingExtension.ValueSplitter));

        await using var dataSource = NpgsqlDataSource.Create(_connectionString);

        await using (var cmd = dataSource.CreateCommand(
                         $"SELECT * FROM {query.Split(DataParsingExtension.QuerySplitter)[2]} WHERE {condition}"))
        {
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                        if (reader.GetFieldType(i) == typeof(string))
                        {
                            if (!reader.IsDBNull(i)) value.Append(reader.GetString(i) + ',');
                            else value.Append("NAN,");
                        }
                        else if (reader.GetFieldType(i) == typeof(int))
                        {
                            if (!reader.IsDBNull(i)) value.Append(reader.GetInt32(i).ToString() + ',');
                            else value.Append("NAN,");
                        }
                        else if (reader.GetFieldType(i) == typeof(bool))
                        {
                            if (!reader.IsDBNull(i)) value.Append(reader.GetBoolean(i).ToString() + ',');
                            else value.Append("NAN,");
                        }

                    if (value.Length == 0) return "ERR";

                    value.Remove(value.Length - 1, 1);
                    return value.ToString();
                }
            }
        }

        if (value.Length == 0)
        {
            query = "ERR";
            return query;
        }

        return value.ToString();
    }

    public async Task<bool> DeleteValueFromAnyTable(String tableName, String condition) //
    {
        await using var dataSource = NpgsqlDataSource.Create(_connectionString);
        try
        {
            // Если удалять несуществующую запись, то ошибки не будет
            await using (var cmd = dataSource.CreateCommand($"DELETE FROM {tableName} WHERE {condition};"))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ew;{ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteValueFromAnyTable(String query) //
    {
        var condition =
            BuildSQLCondition(
                query.Split(DataParsingExtension.QuerySplitter)[2].Split(DataParsingExtension.ValueSplitter),
                query.Split(DataParsingExtension.QuerySplitter)[1].Split(DataParsingExtension.ValueSplitter));

        await using var dataSource = NpgsqlDataSource.Create(_connectionString);
        try
        {
            // Если удалять несуществующую запись, то ошибки не будет
            await using (var cmd = dataSource.CreateCommand(
                             $"DELETE FROM {query.Split(DataParsingExtension.QuerySplitter)[3]} WHERE {condition};"))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ew;{ex.Message}");
            return false;
        }
    }

    public async Task<bool> AddValueToAnyTable(String tableName, String columns, String values) //
    {
        try
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using (var cmd = dataSource.CreateCommand(
                             $"INSERT INTO {tableName} ({columns}) VALUES ({values});"))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ew;{ex.Message}");
            return false;
        }
    }

    public async Task<bool> AddValueToAnyTable(String query) //
    {
        try
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using (var cmd = dataSource.CreateCommand(
                             $"INSERT INTO {query.Split(DataParsingExtension.QuerySplitter)[3]} ({query.Split(DataParsingExtension.QuerySplitter)[2]}) VALUES ({query.Split(DataParsingExtension.QuerySplitter)[1]});"))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ew;{ex.Message}");
            return false;
        }
    }
}