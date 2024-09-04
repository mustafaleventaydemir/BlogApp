using BlogApp.Entity;

namespace BlogApp.Data.Abstract
{
    public interface IUserRepository
    {
        IQueryable<User> Users { get; } //sadece get olacak. ve sadece okuyabileceğiz. 
        void CreateUser(User user);
    }
}
