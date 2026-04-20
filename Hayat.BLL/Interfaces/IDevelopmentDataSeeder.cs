namespace Hayat.BLL.Interfaces
{
    public interface IDevelopmentDataSeeder
    {
        Task SeedAsync(CancellationToken cancellationToken = default);
    }
}
