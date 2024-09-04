using BlogApp.Entity;

namespace BlogApp.Data.Abstract
{
    public interface ICommentRepository
    {
        IQueryable<Comment> Comments { get; } //sadece get olacak. ve sadece okuyabileceğiz. 
        void CreateComment(Comment comment);
    }
}
