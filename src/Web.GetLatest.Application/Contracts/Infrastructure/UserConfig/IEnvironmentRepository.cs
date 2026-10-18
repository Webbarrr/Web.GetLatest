namespace Web.GetLatest.Application.Contracts.Infrastructure.UserConfig;

public interface IEnvironmentRepository
{
    string? GetOwner();
    string? GetPersonalAccessToken();
    string? GetRepo();
}