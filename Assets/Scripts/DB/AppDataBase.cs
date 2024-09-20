using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;


static class AppDataBase
{
    private const string fileName = "db.bytes";
    private static string DBPath;
    public static SqliteConnection connection;
    public static SqliteCommand command;

    static AppDataBase()
    {
        DBPath = GetDatabasePath();
    }

    private static string GetDatabasePath()
    {
    #if UNITY_EDITOR
        return Path.Combine(Application.streamingAssetsPath, fileName);
    #elif UNITY_STANDALONE
        string filePath = Path.Combine(Application.dataPath, fileName);
        if(!File.Exists(filePath)) UnpackDatabase(filePath);
        return filePath;
    #elif UNITY_ANDROID
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if(!File.Exists(filePath)) UnpackDatabase(filePath);
        return filePath;
    #endif
    }

    private static void UnpackDatabase(string toPath)
    {
        string fromPath = Path.Combine(Application.streamingAssetsPath, fileName);

        WWW reader = new WWW(fromPath);
        while (!reader.isDone) { }

        File.WriteAllBytes(toPath, reader.bytes);
    }

    public static void OpenConnection()
    {
        connection = new SqliteConnection("Data Source=" + DBPath);
        command = new SqliteCommand(connection);
        connection.Open();
    }

    public static void CloseConnection()
    {
        connection.Close();
        command.Dispose();
    }

    public static void ExecuteQueryWithoutAnswer(string query)
    {
        OpenConnection();
        command.CommandText = query;
        command.ExecuteNonQuery();
        CloseConnection();
    }

    public static string ExecuteQueryWithAnswer(string query)
    {
        OpenConnection();
        command.CommandText = query;
        var answer = command.ExecuteScalar();
        CloseConnection();
        Debug.Log(answer.ToString());
        if (answer != null) return answer.ToString();
        else return null;
    }

    public static DataTable GetTable(string query)
    {
        OpenConnection();

        SqliteDataAdapter adapter = new SqliteDataAdapter(query, connection);

        DataSet DS = new DataSet();
        adapter.Fill(DS);
        adapter.Dispose();
        CloseConnection();
        if(DS.Tables.Count > 0)
            return DS.Tables[0];
        else
            return null;
    }
}