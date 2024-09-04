using BlogApp.Data.Abstract;
using BlogApp.Entity;

namespace BlogApp.Data.Concrete.EfCore
{
    public class EfCommentRepository : ICommentRepository
    {
        private BlogContext _blogContext;
        public EfCommentRepository(BlogContext blogcontext)
        {
            _blogContext = blogcontext;
        }
        public IQueryable<Comment> Comments => _blogContext.Comments;

        public void CreateComment(Comment comment)
        {
            _blogContext.Comments.Add(comment);
            _blogContext.SaveChanges();
        }
    }
}
