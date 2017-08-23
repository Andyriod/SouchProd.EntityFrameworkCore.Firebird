using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using SouchProd.EntityFrameworkCore.Firebird.FunctionalTests.Models;
using Xunit;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SouchProd.EntityFrameworkCore.Firebird.FunctionalTests.Tests.Connection
{
    public class ConnectionTest
    {
        private static readonly FbConnection Connection = new FbConnection(AppConfig.Config["Data:ConnectionString"]);

        private static AppDb NewDbContext(bool reuseConnection)
        {
            return reuseConnection ? new AppDb(Connection) : new AppDb();
        }

        [Fact]
        public void ContextCreation()
        {
            using (var db = NewDbContext(false))
            {
                db.Database.OpenConnection();
                Assert.Equal("SouchProd.EntityFrameworkCore.Firebird", db.Database.ProviderName);
            }
        }

        [Fact]
        public void RawQueryExecute()
        {
            using (var db = NewDbContext(false))
            {
                var result = db.Database.ExecuteSqlCommand("Select count(*) from rdb$database");
                Assert.Equal(0, result);
            }
        }

        [Fact]
        public void RawQueryRead()
        {
            using (var db = NewDbContext(false))
            using (var command = db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "Select count(*) from rdb$database";
                db.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var i = reader.GetInt32(0);
                        Assert.Equal(1, i);
                    }
                }
            }
        }

        [Fact]
        public void SimpleRowReading()
        {
            using (var db = NewDbContext(false))
            {
                var blog = db.Blogs.FirstOrDefault(x => x.Id > 5);
                Assert.NotNull(blog);

                var blogs = db.Blogs.Where(x => !x.Title.Equals("test")).ToList();
                Assert.NotEqual(0, blogs.Count);

                var item = db.Blogs.Where(x => !x.Title.Equals("test")).ToList();
                Assert.NotEqual(0, item.Count);
            }
        }

        [Fact]
        public async void SimpleRowReadingAsync()
        {
            using (var db = NewDbContext(false))
            {
                var blog = await db.Blogs.FirstOrDefaultAsync();
                Assert.NotNull(blog);
            }
        }

        [Fact]
        public void SimpleUpdate()
        {
            using (var db = NewDbContext(false))
            {
                var blog = db.Blogs.FirstOrDefault();
                blog.Title = "Updated3";
                db.SaveChanges();
                Assert.NotNull(blog);
            }
           
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

    }
}
