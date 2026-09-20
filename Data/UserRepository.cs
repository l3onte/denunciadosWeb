using denunciadosWeb.Models;
using MySqlConnector;

namespace denunciadosWeb.Data;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontro la cadena de conexion."
            );
    }

    public async Task<User?> LoginUserAsync(string name, string password)
    {
        const string sql = @"
            SELECT
                u.id,
                u.nombre,
                u.apellido,
                u.rol_id,
                u.contrasena,
                r.nombre AS rol_nombre
            FROM usuarios u
            INNER JOIN roles r ON r.id = u.rol_id
            WHERE u.nombre = @name;
        ";

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@name", name);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        var passwordHash = reader.GetString("contrasena");

        if (!BCrypt.Net.BCrypt.Verify(password, passwordHash))
        {
            return null;
        }

        return new User
        {
            Id = reader.GetInt32("id"),
            Name = reader.GetString("nombre"),
            LastName = reader.GetString("apellido"),
            RolId = reader.GetInt32("rol_id"),
            Password = passwordHash,

            Role = new Role
            {
                Id = reader.GetInt32("rol_id"),
                Name = reader.GetString("rol_nombre")
            }
        };
    }
}