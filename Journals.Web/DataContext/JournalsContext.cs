using Journals.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Journals.Repository;

namespace Journals.Web.DataContext {
    public class JournalsContext : DbContext, IDisposedTracker {
        public JournalsContext()
            : base("name=JournalsDB") {
        }
        // Entities
        public DbSet<Journal> Journals { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Issue> Issues { get; set; }
        // Entities end
        public bool IsDisposed { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //base.Configuration.LazyLoadingEnabled = false;
            //modelBuilder.Entity<Journal>().ToTable("Journals");
            //modelBuilder.Entity<Subscription>().ToTable("Subscriptions");
            base.OnModelCreating(modelBuilder);
        }
        protected override void Dispose(bool disposing) {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }
}