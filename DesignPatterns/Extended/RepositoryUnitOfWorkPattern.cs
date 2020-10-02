using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DesignPatterns.Extended.RepositoryUW
{
    /*******************************************************************************
     * Repository and Unit of Work (+ Aggregate Root)
     *******************************************************************************
     * What is it?
     * Abstraction of data layer to make data access component usage within
     * applications more easily testable and flexible (easy to switch ORM). A
     * Repository is created for each Domain, and Units of Work combine related
     * Domains into single data access component used by application.
     * 
     * Design Principles (hard to see this implemented cleanly - too many bad demos)
     *  - Abstracts the data access layer away from business components
     *    * Remove dependency of ORM from business layer (easy to test, change ORM)
     *    * Remove complex queries away from business components (repeatable)
     *  - Save/update done only at the unit of work
     *    * Repositories are not IDisposable and have no SaveChanges/Update logic
     *    * Maximize flexibility of the repository to not burden with data operations
     * 
     * Real-world Examples:
     *  - Very common with EF6/Core in MVC and Web API
     *  - Works great with IOC
     * 
     * Demo
     * Create a UOW and Repository pattern to manage data access 
     * 
     *******************************************************************************/

    /*******************************************************************************
     * Repository interface and abstract class (shared to all repositories)
     *******************************************************************************/
    
    // Defines the global generic repository interface
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Find(params object[] keyValues);
        Task<TEntity> FindAsync(params object[] keyValues);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
    
    // Define generic abstract base class
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        public Repository(DbContext context) => _context = context;
        public TEntity Find(params object[] keyValues) => _context.Set<TEntity>().Find(keyValues);
        public async Task<TEntity> FindAsync(params object[] keyValues) => await _context.Set<TEntity>().FindAsync(keyValues);
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate) => _context.Set<TEntity>().Where(predicate);
        public void Add(TEntity entity) => _context.Set<TEntity>().Add(entity);
        public void AddRange(IEnumerable<TEntity> entities) => _context.Set<TEntity>().AddRange(entities);
        public void Remove(TEntity entity) => _context.Set<TEntity>().Remove(entity);
        public void RemoveRange(IEnumerable<TEntity> entities) => _context.Set<TEntity>().RemoveRange(entities);
    }

    // Define shared unit of work contract (disposable)
    public interface IUnitOfWork : IDisposable
    {
        int Complete();
        Task<int> CompleteAsync();
    }

    /*******************************************************************************
     * Implement custom db context (standard entity framework, code first optional
     *******************************************************************************/
     
    public class Author { }
    public class Book { }

    public class MyDbContext : DbContext
    {
        public virtual DbSet<Author> Authors { get; set; } // Virtual
        public virtual DbSet<Book> Books { get; set; } // Virtual
    }

    /*******************************************************************************
     * Implement repositories for each domain model
     *******************************************************************************/

    public interface IAuthorRepository : IRepository<Author>
    { // Shared queries (always IEnumerable, never IQueryable), paging, etc
        IEnumerable<Book> GetBooksByAuthor(int authorId);
        IEnumerable<Book> GetBooks(string search, int? page = null, int? pageSize = null);
    }
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        public AuthorRepository(MyDbContext context) : base(context) { }

        public IEnumerable<Book> GetBooksByAuthor(int authorId) => null; // Not important
        public IEnumerable<Book> GetBooks(string search, int? page = null, int? pageSize) => null; // Take/skip/etc
    }

    public interface IBookRepository : IRepository<Book> { }
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(MyDbContext context) : base(context) { }
    }

    /*******************************************************************************
     * Define the unit of work to include relevant domain repositories
     *******************************************************************************/

    // Disposable, manages save/change of the data
    public interface IBookLibraryUW : IUnitOfWork
    {
        IAuthorRepository Authors { get; }
        IBookRepository Books { get; }
    }

    public class BookLibraryUW : IBookLibraryUW
    {   

        private MyDbContext _context;
        public virtual IAuthorRepository Authors { get; }
        public virtual IBookRepository Books { get; }

        public BookLibraryUW(MyDbContext context)
        {
            _context = context; // Must share same data context
            this.Authors = new AuthorRepository(context);
            this.Books = new BookRepository(context);
        }

        public virtual int Complete() => _context.SaveChanges();
        public virtual async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose(); // Dispose context here

    }

    /*******************************************************************************
     * Sample usage
     *******************************************************************************/

    // Usage in application
    public class DemoController
    {
        private IBookLibraryUW _library;
        public DemoController(IBookLibraryUW library) => _library = library;

        public async Task RemoveBooks(int authorId) // Action or web api endpoint
        {
            var books = _library.Authors.GetBooksByAuthor(authorId);
            _library.Books.RemoveRange(books);
            await _library.CompleteAsync();
        }
    }
}
