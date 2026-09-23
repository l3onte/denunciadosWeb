using denunciadosWeb.Models;
using MySqlConnector;

namespace denunciadosWeb.Data;

public class ComplaintRepository
{
    private readonly string _connectionString;

    public ComplaintRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Can't find the connection string");
    }

    public async Task<List<TypeOfCrime>> GetTypeOfCrime()
    {
        const string sql = @"
            SELECT id, nombre 
            FROM tipos_delito;
        ";

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        var typeOfCrimes = new List<TypeOfCrime>();

        while (await reader.ReadAsync())
        {
            var typeOfCrime = new TypeOfCrime
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("nombre")
            };

            typeOfCrimes.Add(typeOfCrime);
        }

        return typeOfCrimes;
    }

    public async Task<List<LocationType>> GetLocationTypeAsync()
    {
        const string sql = @"
            SELECT id, nombre FROM tipos_lugar;
        ";

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(sql, connection);

        await using var reader = await command.ExecuteReaderAsync();

        var LocationTypes = new List<LocationType>();

        while (await reader.ReadAsync())
        {
            var LocationType = new LocationType
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("nombre")
            };

            LocationTypes.Add(LocationType);
        }

        return LocationTypes;
    }

    public async Task<List<ComplaintStatus>> GetComplaintStatusAsync()
    {
        const string sql = @"
            SELECT id, nombre FROM estados_denuncia;
        ";

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(sql, connection);

        await using var reader = await command.ExecuteReaderAsync();

        var ComplaintStatus = new List<ComplaintStatus>();

        while (await reader.ReadAsync())
        {
            var Status = new ComplaintStatus
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("nombre")
            };

            ComplaintStatus.Add(Status);
        }

        return ComplaintStatus;
    }

    public async Task<List<MunicipalityViewModel>> GetMunicipalityAsync()
    {
        const string sql = @"
            SELECT id, municipio FROM localidades;
        ";

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(sql, connection);

        await using var reader = await command.ExecuteReaderAsync();

        var municipalitys = new List<MunicipalityViewModel>();

        while (await reader.ReadAsync())
        {
            var municipality = new MunicipalityViewModel
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("municipio")
            };

            municipalitys.Add(municipality);
        }

        return municipalitys;
    } 
    public async Task<List<Complaint>> GetComplaintsByFiltersAsync(
        ComplaintFilterViewModel filters)
    {
        const string sql = @"
            SELECT
                d.id AS complaint_id,
                d.fecha_hora_ocurrido,
                d.direccion,
                d.sintesis,
                d.cantidad_masculinos,
                d.cantidad_femeninos,
                d.cantidad_desconocidos,
                d.fecha_hora_registro,

                u.id AS user_id,
                u.nombre AS user_name,
                u.apellido AS user_last_name,

                l.id AS location_id,
                l.nombre AS location_name,
                l.municipio,
                l.numero_distrito,

                tl.id AS location_type_id,
                tl.nombre AS location_type_name,

                td.id AS crime_type_id,
                td.nombre AS crime_type_name,

                e.id AS status_id,
                e.nombre AS status_name

            FROM denuncias d
            INNER JOIN usuarios u ON u.id = d.usuario_id
            INNER JOIN localidades l ON l.id = d.localidad_id
            INNER JOIN tipos_lugar tl ON tl.id = d.tipo_lugar_id
            INNER JOIN tipos_delito td ON td.id = d.tipo_delito_id
            INNER JOIN estados_denuncia e ON e.id = d.estado_id

            WHERE
                (@startDate IS NULL OR d.fecha_hora_ocurrido >= @startDate)
                AND (
                    @endDate IS NULL
                    OR d.fecha_hora_ocurrido < DATE_ADD(@endDate, INTERVAL 1 DAY)
                )
                AND (
                    @locationTypeId IS NULL
                    OR d.tipo_lugar_id = @locationTypeId
                )
                AND (
                    @crimeTypeId IS NULL
                    OR d.tipo_delito_id = @crimeTypeId
                )
                AND (
                    @complaintStatusId IS NULL
                    OR d.estado_id = @complaintStatusId
                )
                AND (
                    @municipality IS NULL
                    OR l.municipio = @municipality
                )
                AND (
                    @location IS NULL
                    OR l.nombre = @location
                )
                AND (
                    @district IS NULL
                    OR l.numero_distrito = @district
                )

            ORDER BY d.fecha_hora_ocurrido DESC;
        ";

        var complaints = new List<Complaint>();

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@startDate",
            filters.StartDate ?? (object)DBNull.Value
        );

        command.Parameters.AddWithValue(
            "@endDate",
            filters.EndDate ?? (object)DBNull.Value
        );

        command.Parameters.AddWithValue(
            "@locationTypeId",
            filters.LocationTypeId ?? (object)DBNull.Value
        );

        command.Parameters.AddWithValue(
            "@crimeTypeId",
            filters.CrimeTypeId ?? (object)DBNull.Value
        );

        command.Parameters.AddWithValue(
            "@complaintStatusId",
            filters.ComplaintStatusId ?? (object)DBNull.Value
        );

        command.Parameters.AddWithValue(
            "@municipality",
            filters.Municipality ?? (object)DBNull.Value
        );

        command.Parameters.AddWithValue(
            "@location",
            filters.Location ?? (object)DBNull.Value
        );

        command.Parameters.AddWithValue(
            "@district",
            filters.District ?? (object)DBNull.Value
        );

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var complaint = new Complaint
            {
                Id = reader.GetInt32("complaint_id"),

                DateAndtimeOfOccurrence =
                    reader.GetDateTime("fecha_hora_ocurrido"),

                Direction = reader.IsDBNull(reader.GetOrdinal("direccion"))
                    ? string.Empty
                    : reader.GetString("direccion"),

                Synthesis = reader.GetString("sintesis"),

                MasculineQuantities =
                    reader.GetInt32("cantidad_masculinos"),

                FamaleQuantities =
                    reader.GetInt32("cantidad_femeninos"),

                UnknownQuantities =
                    reader.GetInt32("cantidad_desconocidos"),

                User = new User
                {
                    Id = reader.GetInt32("user_id"),
                    Name = reader.GetString("user_name"),
                    LastName = reader.GetString("user_last_name")
                },

                Location = new Location
                {
                    Id = reader.GetInt32("location_id"),
                    Name = reader.GetString("location_name"),
                    Municipality = reader.GetString("municipio"),
                    DistrictNumber = reader.GetInt32("numero_distrito")
                },

                LocationType = new LocationType
                {
                    Id = reader.GetInt32("location_type_id"),
                    Name = reader.GetString("location_type_name")
                },

                TypeOfCrime = new TypeOfCrime
                {
                    Id = reader.GetInt32("crime_type_id"),
                    Name = reader.GetString("crime_type_name")
                },

                ComplaintStatus = new ComplaintStatus
                {
                    Id = reader.GetInt32("status_id"),
                    Name = reader.GetString("status_name")
                }
            };

            complaints.Add(complaint);
        }

        return complaints;
    }
}