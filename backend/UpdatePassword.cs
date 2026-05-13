using System;
using Npgsql;

class UpdatePassword
{
    static void Main()
    {
        // BCrypt hash para "pw123456"
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("pw123456");
        
        string connectionString = "Host=localhost;Port=5432;Database=BasePrendeteRock;Username=postgres;Password=Pasteldepapas123#";
        
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("UPDATE \"Users\" SET \"PasswordHash\" = @hash WHERE \"Id\" = 2", conn))
            {
                cmd.Parameters.AddWithValue("hash", passwordHash);
                int rowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine($"Contraseña actualizada. Filas afectadas: {rowsAffected}");
                Console.WriteLine($"Nuevo hash: {passwordHash}");
            }
        }
    }
}
