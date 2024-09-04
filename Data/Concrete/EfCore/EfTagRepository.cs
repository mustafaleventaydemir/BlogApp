using BlogApp.Data.Abstract;
using BlogApp.Entity;

namespace BlogApp.Data.Concrete.EfCore
{
    public class EfTagRepository : ITagRepository
    {
        private BlogContext _blogContext;
        public EfTagRepository(BlogContext blogcontext)
        {
            _blogContext = blogcontext;
        }
        public IQueryable<Tag> Tags => _blogContext.Tags;

        public void CreateTag(Tag tag)
        {
            _blogContext.Tags.Add(tag);
            _blogContext.SaveChanges();
        }
    }
}
