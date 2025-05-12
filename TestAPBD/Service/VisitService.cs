using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TestAPBD.Models;

namespace TestAPBD.Service;

public class VisitService : IVisitsService
{
    
    public readonly string _connectionString;
    
    public VisitService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default");
    }

    public async Task<CustomerIdDTO> getCustomerById(int visistId)
    {
        var result = new CustomerIdDTO();
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var visitExists = await VisitService.visitExists(connection, visistId);

            if (!visitExists)
            {
                throw new Exception("Visit not found");
            }
            
            // Date
            var date = await getDate(connection, visistId);

            var mechanic = await getMechanicById(connection, visistId);
            var client = await getClientById(connection, visistId);


            // Mechanic

            // Visits
            var visits = await getVisits(connection, visistId);
            
            result.date = date;
            result.client = client;
            result.mechanic = mechanic;
            result.visitServices = visits;
            
            return result;
            
        }
    }

    public static async Task<List<VisitServiceDTO>> getVisits(SqlConnection connection, int visistId)
    {
        var sql =
            "select S.name, service_fee from Visit left join dbo.Visit_Service VS on Visit.visit_id = VS.visit_id left join dbo.Service S on S.service_id = VS.service_id where VS.visit_id = @id";
        
        var result = new List<VisitServiceDTO>();

        using (var command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@id", visistId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var visit = new VisitServiceDTO();
                    
                    visit.name = reader.GetString(0);
                    visit.serviceFee = reader.GetDecimal(1);
                    
                    result.Add(visit);
                }
            }
        }
        
        return result;
    }

    public static async Task<String> getDate(SqlConnection connection, int visistId)
    {
        var sql = "select date from Visit where visit_id = @id";
        using (var cmd = new SqlCommand(sql, connection))
        {
            cmd.Parameters.Add(new SqlParameter("@id", visistId));
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToString(result);
        }
    }
    
    public static async Task<bool> visitExists(SqlConnection conn, int visitId)
    {
        string sql = "select 1 from Visit where visit_id = @id";
        using (var cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@id", visitId);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) == 1;
        }
    }

    public static async Task<ClientDTO> getClientById(SqlConnection conn, int visitId)
    {
        string getClientQuery =
            "select first_name, last_name, date_of_birth from Visit left join Client on Visit.client_id = Client.client_id where visit_id = @id";
        ClientDTO result = new ClientDTO();
        
        using (SqlCommand cmd = new SqlCommand(getClientQuery, conn))
        {
            cmd.Parameters.AddWithValue("@id", visitId);


            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    result.firstName = reader.GetString(0);
                    result.lastName = reader.GetString(1);
                    result.dateOfBirth = reader.GetDateTime(2).ToString();
                }
            }
            
        }

        return result;
    }


    public static async Task<MechanicDTO> getMechanicById(SqlConnection conn, int visitId)
    {
        var sql =
            "select M.mechanic_id, licence_number from Visit left join dbo.Mechanic M on M.mechanic_id = Visit.mechanic_id where visit_id = @id";
        var result = new MechanicDTO();
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@id", visitId);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    result.mechanicId = reader.GetInt32(0);
                    result.licenseNumber = reader.GetString(1);
                }
            }

            
        }
        
        return result;
    }
    
    public static async Task<bool> clientExists(SqlConnection conn, int clientId)
    {
        string sql = "select 1 from Client where client_id = @id";
        using (var cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@id", clientId);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) == 1;
        }
    }
    
    public static async Task<bool> mechanicExists(SqlConnection conn, string number)
    {
        string sql = "select 1 from Mechanic where licence_number = @number";
        using (var cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@number", number);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) == 1;
        }
    }

    public static async Task<bool> serviceExists(SqlConnection conn, string serviceName)
    {
        string sql = "select 1 from Service where name = @name;";

        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@name", serviceName);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) == 1;
        }
    }
    
    
    public async Task<int> insertVisits(VisitInsertDTO visit)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            var visitExists = await VisitService.visitExists(conn, visit.visitId);
            if (visitExists)
            {
                throw new Exception("Visit already exists");
            }
            
            var clientExists =  await VisitService.clientExists(conn, visit.clientId);
            if (!clientExists)
            {
                throw new Exception("Client does not exist");
            }

            var mechanicExists = await VisitService.mechanicExists(conn, visit.mechanicalLicenseNumber);
            if (!mechanicExists)
            {
                throw new Exception("Mechanic does not exist");
            }

            foreach (VisitServiceDTO service in visit.visitServices)
            {
                var serviceExists = await VisitService.serviceExists(conn, service.name);
                if (!serviceExists)
                {
                    throw new Exception("Service does not exist");
                }
            }
            
            // Inserting values
            var mechanicId = getMechanicWithLicenseNumber(conn, visit.mechanicalLicenseNumber);
            
            // Get service Ids
            var serviceIds = getServiceIds(conn, visit);
            
            // Create price Map
            var map = new Dictionary<int, decimal>();
            for (int i = 0; i < serviceIds.; i++)
            {
                
            }
            
            var transaction = conn.BeginTransaction();
            await insertVisit(conn, Convert.ToInt32(mechanicId), visit, transaction);
            
            
            
            // Many to many
            //for each
            await insertVisitService()
        }
    }

    public static async Task<List<(int id, decimal price)>> getIdPrices(SqlConnection conn, VisitInsertDTO dto)
    {
        var sql = "select service_id from Service where name = @name";
        var result = new List<(int id, decimal price)>();
        using (var cmd = new SqlCommand(sql, conn))
        {
            foreach (var service in dto.visitServices)
            {
                var obj = cmd.ExecuteScalarAsync();
                int id = Convert.ToInt32(obj);
                result.Add((id, Convert.ToDecimal(obj)));
            }
        }
    }

    public static async Task<int> getServiceId(SqlConnection conn, VisitInsertDTO visit)
    {
        
        int result = 0;
        using (var cmd = new SqlCommand(sql, conn))
        {
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                var id = reader.GetInt32(0);
                result = Convert.ToInt32(id);
            }
        }
        
        return result;
    }

    public static async Task insertVisitService(List<(int id, decimal price)> idPrices, SqlConnection conn, VisitInsertDTO dto)
    {
        var sql =
            "insert into Visit_Service (VISIT_ID, SERVICE_ID, SERVICE_FEE) values (@VISIT_ID, @SERVICE_ID, @SERVICE_FEE)";

        using (var cmd = new SqlCommand(sql, conn))
        {
            foreach (var vale in idPrices)
            {
                cmd.Parameters.AddWithValue("@VISIT_ID", vale.id);
                cmd.Parameters.AddWithValue("@SERVICE_FEE", vale.price);
                cmd.ExecuteNonQuery();
                
            }
        }
    }

    public static async Task insertVisit(SqlConnection conn, int mechanicId, VisitInsertDTO visit, SqlTransaction transaction)
    {
        var sql =
            "insert into Visit (visit_id, client_id, mechanic_id, date) values (@visit_id, @client_id, @mechanic_id, GETDATE())";
        try
        {
            using (var cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@visit_id", visit.visitId);
                cmd.Parameters.AddWithValue("@client_id", visit.clientId);
                cmd.Parameters.AddWithValue("@mechanic_id", mechanicId);

                await cmd.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw ex;
        }
    }

    public static async Task<int> getMechanicWithLicenseNumber(SqlConnection conn, string number)
    {
        var sql = "select top 1 mechanic_id from Mechanic where licence_number = @number";
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@number", number);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }
}