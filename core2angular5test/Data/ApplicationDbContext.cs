using core2angular5test.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace core2angular5test.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        #region constructor
        public ApplicationDbContext(DbContextOptions options) :
            base(options)
        {
            //Database.Migrate();
        }        
        #endregion
        
        #region Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            //TODO: rework to use configuration methods, remove unneeded methods
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<ApplicationUser>().HasMany(u =>
                u.Quizzes).WithOne(i => i.User);
            modelBuilder.Entity<ApplicationUser>().HasMany(u =>
                u.Tokens).WithOne(t => t.User);
            
            modelBuilder.Entity<Quiz>().ToTable("Quizzes");
            modelBuilder.Entity<Quiz>().Property(i =>
                i.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Quiz>().HasOne(i => i.User).WithMany(u
                => u.Quizzes);
            modelBuilder.Entity<Quiz>().HasMany(i =>
                i.Questions).WithOne(c => c.Quiz);
            
            modelBuilder.Entity<Question>().ToTable("Questions");
            modelBuilder.Entity<Question>().Property(i =>
                i.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Question>().HasOne(i =>
                i.Quiz).WithMany(u => u.Questions);
            modelBuilder.Entity<Question>().HasMany(i =>
                i.Answers).WithOne(c => c.Question);
            
            modelBuilder.Entity<Answer>().ToTable("Answers");
            modelBuilder.Entity<Answer>().Property(i =>
                i.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Answer>().HasOne(i =>
                i.Question).WithMany(u => u.Answers);
            
            modelBuilder.Entity<Result>().ToTable("Results");
            modelBuilder.Entity<Result>().Property(i =>
                i.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Result>().HasOne(i =>
                i.Quiz).WithMany(u => u.Results);
            
            modelBuilder.Entity<Token>().ToTable("Tokens");
            //TODO: Is this needed if Key attribute has already been specified?
            modelBuilder.Entity<Token>().Property(i => i.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Token>().HasOne(i => i.User).WithMany(u => u.Tokens);
        }
        
        #endregion
        
        #region Properties        
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Token> Tokens { get; set; }
        #endregion Properties
    }
}