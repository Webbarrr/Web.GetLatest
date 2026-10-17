namespace Web.GetLatest.Application.Contracts.Infrastructure.UserConfig;

public interface IUserConfigRepository
{
    UserConfigResponse? LoadConfig();
}
