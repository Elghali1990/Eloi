using Microsoft.EntityFrameworkCore.Metadata;

namespace E.Loi.Services.DB;

public partial class LawDbContext : DbContext
{
    public LawDbContext()
    {
    }

    public LawDbContext(DbContextOptions<LawDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Amendment> Amendments { get; set; }
    public virtual DbSet<AmendmentContent> AmendmentContents { get; set; }
    public virtual DbSet<Datum> Data { get; set; }
    public virtual DbSet<Document> Documents { get; set; }
    public virtual DbSet<GovernmentPosition> GovernmentPositions { get; set; }
    public virtual DbSet<Law> Laws { get; set; }
    public virtual DbSet<Legislative> Legislatives { get; set; }
    public virtual DbSet<LegislativeSession> LegislativeSessions { get; set; }
    public virtual DbSet<LegislativeYear> LegislativeYears { get; set; }
    public virtual DbSet<Node> Nodes { get; set; }
    public virtual DbSet<NodeHierarchyFamilly> NodeHierarchyFamillies { get; set; }
    public virtual DbSet<NodeType> NodeTypes { get; set; }
    public virtual DbSet<Phase> Phases { get; set; }
    public virtual DbSet<PhaseLawId> PhaseLawIds { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Statut> Statuts { get; set; }
    public virtual DbSet<Team> Teams { get; set; }
    public virtual DbSet<Trace> Traces { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<VoteAmendementResult> VoteAmendementResults { get; set; }
    public virtual DbSet<VoteNodeResult> VoteNodeResults { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder
    //    .UseSqlServer("Data Source=192.168.1.23;Database=EloisNew;User Id=sa;password=Dev@2022;Trusted_Connection=False;MultipleActiveResultSets=false;TrustServerCertificate=True");

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //=> optionsBuilder
    //.UseSqlServer("Data Source=192.168.1.29;Database=EloisNew;User Id=sa;password=Pa$$w0rd;Trusted_Connection=False;MultipleActiveResultSets=false;TrustServerCertificate=True");

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsbuilder)
    //    => optionsbuilder.UseSqlServer("Data Source=192.168.1.17;Database=EloisNew;User Id=Plf;password=%87!pro;Trusted_Connection=False;MultipleActiveResultSets=false;TrustServerCertificate=True");
    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        modelBuilder.Entity<Amendment>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("AmendmentsTrigger"));
            entity.HasIndex(e => e.CreatedFromId, "IX_Amendments_CreatedFromId");
            entity.HasIndex(e => e.NodeId, "IX_Amendments_NodeId");
            entity.HasIndex(e => e.ReferenceNodeId, "IX_Amendments_ReferenceNodeId");
            entity.HasIndex(e => e.TeamId, "IX_Amendments_TeamId");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.AmendmentIntent).HasMaxLength(20);
            entity.Property(e => e.AmendmentType).HasMaxLength(30);
            entity.Property(e => e.AmendmentsStatus).HasMaxLength(10);
            entity.Property(e => e.ArticleRef).HasMaxLength(30);
            entity.Property(e => e.NumSystem).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            entity.Property(e => e.OldNodeId).HasColumnName("oldNodeId");
            entity.Property(e => e.OrderParagraphe)
            .HasDefaultValue(0)
            .HasColumnName("orderParagraphe");
            entity.Property(e => e.TitreParagraphe).HasMaxLength(50);
            entity.Property(e => e.PublishedDate).HasColumnType("datetime");
            entity.HasOne(d => d.CreatedFrom).WithMany(p => p.InverseCreatedFrom).HasForeignKey(d => d.CreatedFromId);
            entity.HasOne(d => d.Team).WithMany(p => p.Amendments)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasMany(d => d.Amendments).WithMany(p => p.AmendmentClusters)
                .UsingEntity<Dictionary<string, object>>(
                    "AmendmentCluster",
                    r => r.HasOne<Amendment>().WithMany()
                        .HasForeignKey("AmendmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Amendment>().WithMany()
                        .HasForeignKey("RefAmendmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("AmendmentId", "RefAmendmentId");
                        j.ToTable("AmendmentClusters");
                        j.HasIndex(new[] { "RefAmendmentId" }, "IX_AmendmentClusters_RefAmendmentId");
                    });

            entity.HasMany(d => d.AmendmentClusters).WithMany(p => p.Amendments)
                .UsingEntity<Dictionary<string, object>>(
                    "AmendmentCluster",
                    r => r.HasOne<Amendment>().WithMany()
                        .HasForeignKey("RefAmendmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Amendment>().WithMany()
                        .HasForeignKey("AmendmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("AmendmentId", "RefAmendmentId");
                        j.ToTable("AmendmentClusters");
                        j.HasIndex(new[] { "RefAmendmentId" }, "IX_AmendmentClusters_RefAmendmentId");
                    });
        });


        modelBuilder.Entity<AmendmentContent>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Amendment).WithMany(p => p.AmendmentContents)
                .HasForeignKey(d => d.AmendmentId)
                .HasConstraintName("FK_AmendmentContents_Amendments");
        });
        modelBuilder.Entity<Datum>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.Id).HasColumnName("ID");
        });
        modelBuilder.Entity<Document>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Path).HasMaxLength(250);
            entity.Property(e => e.Type).HasMaxLength(25);
        });
        modelBuilder.Entity<GovernmentPosition>(entity =>
        {
            entity.HasIndex(e => e.AmendmentId, "IX_GovernmentPositions_AmendmentId");
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Position).HasMaxLength(10);
            entity.HasOne(d => d.Amendment).WithMany(p => p.GovernmentPositions)
                .HasForeignKey(d => d.AmendmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        modelBuilder.Entity<Law>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Category).HasMaxLength(20);
            entity.Property(e => e.DateAffectationBureau2)
                .HasColumnType("datetime")
                .HasColumnName("DateAffectationBureau_2");
            entity.Property(e => e.DateAffectationCdc).HasColumnName("DateAffectationCDC");
            entity.Property(e => e.DateFinAmendments1).HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            entity.Property(e => e.DateFinAmendments2).HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            entity.Property(e => e.DateVoteCommRead1)
                .HasColumnType("datetime")
                .HasColumnName("DateVoteCommRead_1");
            entity.Property(e => e.DateVoteCommRead2)
                .HasColumnType("datetime")
                .HasColumnName("DateVoteCommRead_2");
            entity.Property(e => e.DateVoteSeanceRead1)
                .HasColumnType("datetime")
                .HasColumnName("DateVoteSeanceRead_1");
            entity.Property(e => e.DateVoteSeanceRead2)
                .HasColumnType("datetime")
                .HasColumnName("DateVoteSeanceRead_2");
            entity.Property(e => e.Number).HasMaxLength(10);
            entity.Property(e => e.ProgrammedDateCommRead1)
                .HasColumnType("datetime")
                .HasColumnName("ProgrammedDateCommRead_1");
            entity.Property(e => e.ProgrammedDateCommRead2)
                .HasColumnType("datetime")
                .HasColumnName("ProgrammedDateCommRead_2");
            entity.Property(e => e.TextOwner).HasMaxLength(150);
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .HasColumnName("type");

            entity.HasOne(d => d.Commission).WithMany(p => p.LawIdCommissionNavigations)
                .HasForeignKey(d => d.IdCommission)
                .HasConstraintName("FK_Laws_Teams");

            entity.HasOne(d => d.Team).WithMany(p => p.LawIdEquipeNavigations)
                .HasForeignKey(d => d.IdEquipe)
                .HasConstraintName("FK_Laws_Teams1");

            entity.HasOne(d => d.IdLegislativeNavigation).WithMany(p => p.Laws)
                .HasForeignKey(d => d.IdLegislative)
                .HasConstraintName("FK_Laws_Legislatives");

            entity.HasOne(d => d.IdSessionNavigation).WithMany(p => p.Laws)
                .HasForeignKey(d => d.IdSession)
                .HasConstraintName("FK_Laws_LegislativeSession");

            entity.HasOne(d => d.NodeHierarchyFamilly).WithMany(p => p.Laws).HasForeignKey(d => d.NodeHierarchyFamillyId);
            entity.HasOne(d => d.PhaseLaw).WithMany(p => p.Laws).HasForeignKey(d => d.PhaseLawId).HasConstraintName("FK_Laws_Phases");

            entity.HasOne(d => d.PhaseLaw).WithMany(p => p.Laws)
                .HasForeignKey(d => d.PhaseLawId)
                .HasConstraintName("FK_Laws_Phases");

            entity.HasMany(d => d.Laws).WithMany(p => p.RefLaws)
                .UsingEntity<Dictionary<string, object>>(
                    "MappedLaw",
                    r => r.HasOne<Law>().WithMany()
                        .HasForeignKey("LawId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Law>().WithMany()
                        .HasForeignKey("RefLawId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("LawId", "RefLawId");
                        j.ToTable("MappedLaws");
                    });

            entity.HasMany(d => d.RefLaws).WithMany(p => p.Laws)
                .UsingEntity<Dictionary<string, object>>(
                    "MappedLaw",
                    r => r.HasOne<Law>().WithMany()
                        .HasForeignKey("RefLawId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Law>().WithMany()
                        .HasForeignKey("LawId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("LawId", "RefLawId");
                        j.ToTable("MappedLaws");
                    });
        });
        modelBuilder.Entity<Legislative>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Legislative");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Label).HasMaxLength(100);
        });
        modelBuilder.Entity<LegislativeSession>(entity =>
        {
            entity.ToTable("LegislativeSession");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Label).HasMaxLength(100);

            entity.HasOne(d => d.IdYearNavigation).WithMany(p => p.LegislativeSessions)
                .HasForeignKey(d => d.IdYear)
                .HasConstraintName("FK_LegislativeSession_LegislativeYears");
        });
        modelBuilder.Entity<LegislativeYear>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Label).HasMaxLength(100);

            entity.HasOne(d => d.Legislative).WithMany(p => p.LegislativeYears)
                .HasForeignKey(d => d.LegislativeId)
                .HasConstraintName("FK_LegislativeYears_Legislatives");
        });
        modelBuilder.Entity<Node>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Bis).HasColumnName("bis");
            entity.Property(e => e.Nature)
                .HasMaxLength(10)
                .HasColumnName("nature");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.OldContent).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Order).HasColumnName("order");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");

            entity.HasOne(d => d.Law).WithMany(p => p.Nodes)
                .HasForeignKey(d => d.LawId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PhaseLaw).WithMany(p => p.Nodes)
                .HasForeignKey(d => d.PhaseLawId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Type).WithMany(p => p.Nodes)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        modelBuilder.Entity<NodeHierarchyFamilly>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });
        modelBuilder.Entity<NodeType>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ContentType)
                .HasMaxLength(20)
                .HasColumnName("contentType");
            entity.Property(e => e.HasContent).HasColumnName("hasContent");
            entity.Property(e => e.HasOriginalContent).HasColumnName("hasOriginalContent");
            entity.Property(e => e.HasPresentationNote).HasColumnName("hasPresentationNote");
            entity.Property(e => e.HasTitle).HasColumnName("hasTitle");
            entity.Property(e => e.IsAmendableAdd)
                .HasDefaultValue(true)
                .HasColumnName("isAmendableAdd");
            entity.Property(e => e.IsAmendableDelete)
                .HasDefaultValue(true)
                .HasColumnName("isAmendableDelete");
            entity.Property(e => e.IsAmendableEdit)
                .HasDefaultValue(true)
                .HasColumnName("isAmendableEdit");
            entity.Property(e => e.IsParentType).HasColumnName("isParentType");
            entity.Property(e => e.IsVotable)
                .HasDefaultValue(true)
                .HasColumnName("isVotable");
            entity.Property(e => e.Label).HasColumnName("label");
            entity.Property(e => e.OrderType)
                .HasMaxLength(20)
                .HasColumnName("orderType");
            entity.Property(e => e.TextType).HasMaxLength(20);
            entity.HasOne(d => d.Familly).WithMany(p => p.NodeTypes)
                .HasForeignKey(d => d.FamillyId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasMany(d => d.Children).WithMany(p => p.Parents)
                .UsingEntity<Dictionary<string, object>>(
                    "NodeHierarchy",
                    r => r.HasOne<NodeType>().WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<NodeType>().WithMany()
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("ParentId", "ChildId");
                        j.ToTable("NodeHierarchies");
                    });

            entity.HasMany(d => d.Parents).WithMany(p => p.Children)
                .UsingEntity<Dictionary<string, object>>(
                    "NodeHierarchy",
                    r => r.HasOne<NodeType>().WithMany()
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<NodeType>().WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("ParentId", "ChildId");
                        j.ToTable("NodeHierarchies");
                    });
        });
        modelBuilder.Entity<Phase>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });
        modelBuilder.Entity<PhaseLawId>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.PhaseId).HasColumnName("PhaseId");
            entity.Property(e => e.Statu).HasMaxLength(50);

            entity.HasOne(d => d.Law).WithMany(p => p.PhaseLawIds)
                .HasForeignKey(d => d.LawId)
                .HasConstraintName("FK_PhaseLawIds_Laws");

            entity.HasOne(d => d.Phases).WithMany(p => p.PhaseLawIds)
                .HasForeignKey(d => d.PhaseId)
                .HasConstraintName("FK_PhaseLawIds_Phases");
        });
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasMany(d => d.Users).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserRole_Users"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserRole_Role"),
                    j =>
                    {
                        j.HasKey("RoleId", "UserId");
                        j.ToTable("UserRole");
                    });
        });
        modelBuilder.Entity<Statut>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Label).HasMaxLength(150);
        });
        modelBuilder.Entity<Team>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.TeamEntity)
                .HasMaxLength(30)
                .HasDefaultValue("");
            entity.Property(e => e.TeamType).HasMaxLength(30);

            entity.HasMany(d => d.MemberTeams).WithMany(p => p.ParentTeams)
                .UsingEntity<Dictionary<string, object>>(
                    "TeamCluster",
                    r => r.HasOne<Team>().WithMany()
                        .HasForeignKey("MemberTeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Team>().WithMany()
                        .HasForeignKey("ParentTeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("ParentTeamId", "MemberTeamId");
                        j.ToTable("TeamClusters");
                    });

            entity.HasMany(d => d.ParentTeams).WithMany(p => p.MemberTeams)
                .UsingEntity<Dictionary<string, object>>(
                    "TeamCluster",
                    r => r.HasOne<Team>().WithMany()
                        .HasForeignKey("ParentTeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Team>().WithMany()
                        .HasForeignKey("MemberTeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("ParentTeamId", "MemberTeamId");
                        j.ToTable("TeamClusters");
                    });
        });
        modelBuilder.Entity<Trace>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DateOperation).HasColumnType("datetime");
            entity.Property(e => e.Operation).HasMaxLength(100);
        });
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Hash).HasMaxLength(400);
            entity.Property(e => e.Occupation).HasMaxLength(25);
            entity.Property(e => e.Salt)
                .HasMaxLength(400)
                .HasColumnName("salt");
            entity.Property(e => e.Structure).HasMaxLength(25);

            entity.HasOne(d => d.Team).WithMany(p => p.Users)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_Users_Teams");

            entity.HasMany(d => d.Teams).WithMany(p => p.UsersNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "UserTeam",
                    r => r.HasOne<Team>().WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserTeams_Teams"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserTeams_Users"),
                    j =>
                    {
                        j.HasKey("UserId", "TeamId");
                        j.ToTable("UserTeams");
                    });
        });
        modelBuilder.Entity<VoteAmendementResult>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Result).HasMaxLength(10);

            //entity.HasOne(d => d.Amendment).WithOne(p => p.VoteAmendementResult)
            //    .HasForeignKey<Amendment>(d=>d.Id)
            //    .OnDelete(DeleteBehavior.ClientSetNull);
        });
        modelBuilder.Entity<VoteNodeResult>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Result).HasMaxLength(10);

            // entity.HasOne(d => d.NodePhaseLaw).WithMany(p => p.VoteNodeResults).HasForeignKey(d => d.NodePhaseLawId);
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
