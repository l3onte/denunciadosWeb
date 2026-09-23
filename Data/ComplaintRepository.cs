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

    public async Task<List<Location>> GetLocationsAsync()
    {
        const string sql = @"
            SELECT 
                id,
                nombre,
                municipio,
                numero_distrito
            FROM localidades
            ORDER BY municipio, nombre;
        ";

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        var locations = new List<Location>();

        while (await reader.ReadAsync())
        {
            var location = new Location
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("nombre"),
                Municipality = reader.GetString("municipio"),
                DistrictNumber = reader.GetInt32("numero_distrito")
            };

            locations.Add(location);
        }

        return locations;
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
    
   public async Task<int> CreateComplaintAsync(
    CreateComplaintViewModel model,
    int userId)
    {
        const string complaintSql = @"
            INSERT INTO denuncias
            (
                usuario_id,
                localidad_id,
                tipo_lugar_id,
                tipo_delito_id,
                estado_id,
                fecha_hora_ocurrido,
                direccion,
                sintesis,
                cantidad_masculinos,
                cantidad_femeninos,
                cantidad_desconocidos
            )
            VALUES
            (
                @userId,
                @locationId,
                @locationTypeId,
                @crimeTypeId,
                @complaintStatusId,
                @dateAndTimeOfOccurrence,
                @direction,
                @synthesis,
                @masculineQuantities,
                @femaleQuantities,
                @unknownQuantities
            );

            SELECT LAST_INSERT_ID();
        ";

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var masculineQuantities = model.Victims
                .Count(v => v.Sex == "Masculino");

            var femaleQuantities = model.Victims
                .Count(v => v.Sex == "Femenino");

            var unknownQuantities = model.Victims
                .Count(v =>
                    string.IsNullOrWhiteSpace(v.Sex) ||
                    v.Sex == "Desconocido"
                );

            await using var complaintCommand = new MySqlCommand(
                complaintSql,
                connection,
                transaction
            );

            complaintCommand.Parameters.AddWithValue(
                "@userId",
                userId
            );

            complaintCommand.Parameters.AddWithValue(
                "@locationId",
                model.LocationId!.Value
            );

            complaintCommand.Parameters.AddWithValue(
                "@locationTypeId",
                model.LocationTypeId!.Value
            );

            complaintCommand.Parameters.AddWithValue(
                "@crimeTypeId",
                model.CrimeTypeId!.Value
            );

            complaintCommand.Parameters.AddWithValue(
                "@complaintStatusId",
                model.ComplaintStatusId!.Value
            );

            complaintCommand.Parameters.AddWithValue(
                "@dateAndTimeOfOccurrence",
                model.DateAndTimeOfOccurrence!.Value
            );

            complaintCommand.Parameters.AddWithValue(
                "@direction",
                model.Direction ?? (object)DBNull.Value
            );

            complaintCommand.Parameters.AddWithValue(
                "@synthesis",
                model.Synthesis
            );

            complaintCommand.Parameters.AddWithValue(
                "@masculineQuantities",
                masculineQuantities
            );

            complaintCommand.Parameters.AddWithValue(
                "@femaleQuantities",
                femaleQuantities
            );

            complaintCommand.Parameters.AddWithValue(
                "@unknownQuantities",
                unknownQuantities
            );

            var result = await complaintCommand.ExecuteScalarAsync();

            var complaintId = Convert.ToInt32(result);

            foreach (var victim in model.Victims)
            {
                const string sql = @"
                    INSERT INTO victimas
                    (
                        denuncia_id,
                        nombre,
                        apellido,
                        identificacion,
                        sexo,
                        edad
                    )
                    VALUES
                    (
                        @complaintId,
                        @name,
                        @lastName,
                        @identification,
                        @sex,
                        @age
                    );
                ";

                await using var command = new MySqlCommand(
                    sql,
                    connection,
                    transaction
                );

                command.Parameters.AddWithValue(
                    "@complaintId",
                    complaintId
                );

                command.Parameters.AddWithValue(
                    "@name",
                    victim.Name
                );

                command.Parameters.AddWithValue(
                    "@lastName",
                    victim.LastName
                );

                command.Parameters.AddWithValue(
                    "@identification",
                    victim.Identification ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@sex",
                    victim.Sex ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@age",
                    victim.Age ?? (object)DBNull.Value
                );

                await command.ExecuteNonQueryAsync();
            }

            foreach (var witness in model.Witnesses)
            {
                const string sql = @"
                    INSERT INTO testigos
                    (
                        denuncia_id,
                        nombre,
                        apellido,
                        identificacion,
                        sexo,
                        edad
                    )
                    VALUES
                    (
                        @complaintId,
                        @name,
                        @lastName,
                        @identification,
                        @sex,
                        @age
                    );
                ";

                await using var command = new MySqlCommand(
                    sql,
                    connection,
                    transaction
                );

                command.Parameters.AddWithValue(
                    "@complaintId",
                    complaintId
                );

                command.Parameters.AddWithValue(
                    "@name",
                    witness.Name
                );

                command.Parameters.AddWithValue(
                    "@lastName",
                    witness.LastName
                );

                command.Parameters.AddWithValue(
                    "@identification",
                    witness.Identification ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@sex",
                    witness.Sex ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@age",
                    witness.Age ?? (object)DBNull.Value
                );

                await command.ExecuteNonQueryAsync();
            }

            foreach (var author in model.PresumedAuthors)
            {
                const string sql = @"
                    INSERT INTO presuntos_autores
                    (
                        denuncia_id,
                        nombre,
                        apellido,
                        alias,
                        descripcion,
                        color_piel,
                        estatura_aproximada,
                        cabello,
                        contextura,
                        sexo,
                        tatuajes,
                        cicatrices
                    )
                    VALUES
                    (
                        @complaintId,
                        @name,
                        @lastName,
                        @alias,
                        @description,
                        @skinColor,
                        @approximateHeight,
                        @hair,
                        @build,
                        @sex,
                        @tattoos,
                        @scars
                    );
                ";

                await using var command = new MySqlCommand(
                    sql,
                    connection,
                    transaction
                );

                command.Parameters.AddWithValue(
                    "@complaintId",
                    complaintId
                );

                command.Parameters.AddWithValue(
                    "@name",
                    author.Name ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@lastName",
                    author.LastName ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@alias",
                    author.Alias ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@description",
                    author.Description ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@skinColor",
                    author.SkinColor ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@approximateHeight",
                    author.ApproximateHeight ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@hair",
                    author.Hair ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@build",
                    author.Build ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@sex",
                    author.Sex ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@tattoos",
                    author.Tattoos ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@scars",
                    author.Scars ?? (object)DBNull.Value
                );

                await command.ExecuteNonQueryAsync();
            }

            foreach (var affectedObject in model.AffectedObjects)
            {
                const string sql = @"
                    INSERT INTO objetos_afectados
                    (
                        denuncia_id,
                        nombre,
                        descripcion,
                        cantidad
                    )
                    VALUES
                    (
                        @complaintId,
                        @name,
                        @description,
                        @quantity
                    );
                ";

                await using var command = new MySqlCommand(
                    sql,
                    connection,
                    transaction
                );

                command.Parameters.AddWithValue(
                    "@complaintId",
                    complaintId
                );

                command.Parameters.AddWithValue(
                    "@name",
                    affectedObject.Name
                );

                command.Parameters.AddWithValue(
                    "@description",
                    affectedObject.Description ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@quantity",
                    affectedObject.Quantity
                );

                await command.ExecuteNonQueryAsync();
            }

            foreach (var usedObject in model.UsedObjects)
            {
                const string sql = @"
                    INSERT INTO objetos_utilizados
                    (
                        denuncia_id,
                        nombre,
                        descripcion,
                        cantidad
                    )
                    VALUES
                    (
                        @complaintId,
                        @name,
                        @description,
                        @quantity
                    );
                ";

                await using var command = new MySqlCommand(
                    sql,
                    connection,
                    transaction
                );

                command.Parameters.AddWithValue(
                    "@complaintId",
                    complaintId
                );

                command.Parameters.AddWithValue(
                    "@name",
                    usedObject.Name
                );

                command.Parameters.AddWithValue(
                    "@description",
                    usedObject.Description ?? (object)DBNull.Value
                );

                command.Parameters.AddWithValue(
                    "@quantity",
                    usedObject.Quantity
                );

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();

            return complaintId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}