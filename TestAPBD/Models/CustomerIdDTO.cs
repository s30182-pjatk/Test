namespace TestAPBD.Models;

public class CustomerIdDTO
{
    public string date { get; set; }
    public ClientDTO client { get; set; }
    public MechanicDTO mechanic { get; set; }
    public List<VisitServiceDTO> visitServices { get; set; }
    
}

public class ClientDTO
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string dateOfBirth {get; set;}
}

public class MechanicDTO
{
    public int mechanicId { get; set; }
    public string licenseNumber { get; set; }
}

public class VisitServiceDTO
{
    public string name { get; set; }
    public decimal serviceFee { get; set; }
}

public class VisitInsertDTO
{
    public int visitId { get; set; }
    public int clientId { get; set; }
    public string mechanicalLicenseNumber { get; set; }
    public List<VisitServiceDTO> visitServices { get; set; }
}