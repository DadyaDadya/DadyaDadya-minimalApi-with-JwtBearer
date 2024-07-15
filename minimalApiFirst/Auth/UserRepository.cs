namespace minimalApiFirst.Auth
{
    public class UserRepository : IUserRepository
    {
        private List<UserDto> users = new()
        {
            new UserDto ("1","1"),
            new UserDto ("2","2"),            
            new UserDto ("3","3"),
        };
        public UserDto GetUser(UserModel model)
        {
            return users.FirstOrDefault(x => x.username == model.UserName && x.password == model.Password) ?? throw new Exception();
        }
    }
}
