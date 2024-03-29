﻿using IdGen;
using System.Text.Json;

namespace PaymentGateway.Ultils.Extension;

public static class Extension
{
    private static readonly IdGenerator _idGenerator = new IdGenerator(0);
    private static readonly JsonSerializerOptions serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static T GetObject<T>(this JsonElement json)
    {
        return json.Deserialize<T>(serializerOptions)!;
    }

    public static async Task<List<T?>> TaskToValue<T>(this List<Task<T?>> list)
    {
        await Task.WhenAll(list);
        var result = new List<T?>(list.Count);
        foreach (var item in list) result.Add(await item);
        return result;
    }

    public static string GetInsertQuery(string table, params string[] props)
    {
        string key = string.Join(", ", props);
        string value = $"@{string.Join(", @", props)}";
        string query = @$"INSERT INTO {table}({key}) VALUES({value});";
        return query;
    }

    public static string GetUpdateQuery(string table, string id, params string[] props)
    {
        if (props == null || props.Length == 0)
        {
            throw new ArgumentException("Phải cung cấp ít nhất một thuộc tính.");
        }
        string keyValues = string.Join(", ", props.Select(prop => $"{prop} = @{prop}"));
        string query = @$"UPDATE {table} SET {keyValues} WHERE {id} = @{id};";
        return query;
    }

    private static readonly Random _random = new Random();
    public static string RamdomNumber()
    {
        char[] chars = new char[6];
        for (int i = 0; i < 6; i++)
        {
            chars[i] = (char)(_random.Next(0, 10) + '0');
        }
        return new string(chars);
    }
    public static long GenerateUniqueId()
    {
        return _idGenerator.CreateId();
    }
}