using TestAPBD.Models;

namespace TestAPBD.Service;

public interface IVisitsService
{
    Task<CustomerIdDTO> getCustomerById(int customerId);
    Task<int> insertVisits(VisitInsertDTO visit);
}