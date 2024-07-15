namespace minimalApiFirst.Auth
{
    public interface IUserRepository
    {
        UserDto GetUser(UserModel model);
    }
}
