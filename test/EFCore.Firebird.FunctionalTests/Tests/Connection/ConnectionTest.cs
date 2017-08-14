using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using SouchProd.EntityFrameworkCore.Firebird.FunctionalTests.Models;
using Xunit;
using System.Linq;

namespace SouchProd.EntityFrameworkCore.Firebird.FunctionalTests.Tests.Connection
{
    public class ConnectionTest
    {
        private static readonly FbConnection Connection = new FbConnection(AppConfig.Config["Data:ConnectionString"]);

        private static AppDb NewDbContext(bool reuseConnection)
        {
            return reuseConnection ? new AppDb(Connection) : new AppDb();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AffectedRowsFalse(bool reuseConnection)
        {
            var title = "test";
            var blog = new Blog { Title = title };
            using (var db = NewDbContext(reuseConnection))
            {
                db.Blogs.Add(blog);
                db.SaveChanges();
            }
            Assert.True(blog.Id > 0);

            // this will throw a DbUpdateConcurrencyException if UseAffectedRows=true
            var sameBlog = new Blog { Id = blog.Id, Title = title };
            using (var db = NewDbContext(reuseConnection))
            {
                db.Blogs.Update(sameBlog);
                await db.SaveChangesAsync();
            }
            Assert.Equal(blog.Id, sameBlog.Id);
        }

        [Fact]
        public void SimpleRowReading()
        {
            using (var db = NewDbContext(false))
            {
                var blog = db.Blogs.FirstOrDefault();
                Assert.NotNull(blog);
            }                     
        }
    }
}
