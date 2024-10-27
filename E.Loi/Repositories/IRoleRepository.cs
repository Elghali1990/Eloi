namespace E.Loi.Repositories;

public interface IRoleRepository
{
    Task<List<Role>> GetAll();
}
