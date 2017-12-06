namespace DataAccess
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using DataAccesLayer.Migrations;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public partial class CalendarDBContext : DbContext
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<ColorE> Colors { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<EMailAddress> EMailAddresses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<EventApproval> EventApprovals { get; set; }
        public CalendarDBContext()
            : base("name=CalendarDBContext")
        {
           Database.SetInitializer(new MigrateDatabaseToLatestVersion<CalendarDBContext, Configuration>("CalendarDBContext"));
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Entity<Plan>()
                .HasMany<Event>(p => p.Events)
                .WithMany(e => e.Plans)
                .Map(pe =>
                {
                    pe.MapLeftKey("PlanId");
                    pe.MapRightKey("EventId");
                    pe.ToTable("PlanEvent");
                });
            modelBuilder.Entity<User>()
                .HasMany<Plan>(u => u.Plans)
                .WithMany(p => p.Users)
                .Map(up =>
                {
                    up.MapLeftKey("UserId");
                    up.MapRightKey("PlanId");
                    up.ToTable("PlanUsers");
                 });
            modelBuilder.Entity<Event>()
                .HasOptional(t => t.Type);
            modelBuilder.Entity<Type>()
                .HasRequired(t => t.Color);

        }
    }
}
