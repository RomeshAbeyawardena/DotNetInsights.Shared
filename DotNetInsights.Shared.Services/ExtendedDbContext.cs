using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DotNetInsights.Shared.Services
{
    public abstract class ExtendedDbContext : DbContext
    {
        private readonly bool useSingularTableNames;

        public ExtendedDbContext(bool useSingularTableNames = true)
        {
            this.useSingularTableNames = useSingularTableNames;
        }

        public ExtendedDbContext(DbContextOptions dbContextOptions, bool useSingularTableNames = true)
            : base(dbContextOptions)
        {
            this.useSingularTableNames = useSingularTableNames;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (useSingularTableNames)
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                    SetTableName(entityType);
        }

        private void SetTableName(IMutableEntityType mutableEntityType)
        {
            mutableEntityType.SetTableName(mutableEntityType.GetTableName().Singularize());
        }
    }
}
