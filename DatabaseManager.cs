using System.Data.SQLite;

public class DatabaseManager
{
    private static string connectionString = "Data Source=puzzles.db;Version=3;";

    public static void InitializeDatabase()
    {
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();

        string sql = @"
            CREATE TABLE IF NOT EXISTS puzzles (
                board_data BLOB
            )";
        using var cmd = new SQLiteCommand(sql, connection);
        cmd.ExecuteNonQuery();
    }

    public static void SavePuzzleToDatabase(byte[] boardData, SQLiteConnection connection, SQLiteTransaction transaction)
    {
        string sql = "INSERT INTO puzzles (board_data) VALUES (@data)";
        using var cmd = new SQLiteCommand(sql, connection, transaction);
        cmd.Parameters.AddWithValue("@data", boardData);
        cmd.ExecuteNonQuery();
    }

    public Board GetRandomPuzzle()
    {
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();

        string sql = "SELECT board_data FROM puzzles ORDER BY RANDOM() LIMIT 1";
        using var cmd = new SQLiteCommand(sql, connection);

        byte[] data = (byte[])cmd.ExecuteScalar();
        return Transformer.FromHuffmanBinary(data);
    }
    
}