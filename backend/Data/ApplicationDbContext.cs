using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using backend.Models.Entities;

namespace backend.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Community> Communities { get; set; }

    public virtual DbSet<CommunityJoinRequest> CommunityJoinRequests { get; set; }

    public virtual DbSet<CommunityResident> CommunityResidents { get; set; }

    public virtual DbSet<DirectMessage> DirectMessages { get; set; }

    public virtual DbSet<EcoPointTransaction> EcoPointTransactions { get; set; }

    public virtual DbSet<Initiative> Initiatives { get; set; }

    public virtual DbSet<InitiativeCategory> InitiativeCategories { get; set; }

    public virtual DbSet<InitiativeComment> InitiativeComments { get; set; }

    public virtual DbSet<InitiativeCommentLike> InitiativeCommentLikes { get; set; }

    public virtual DbSet<InitiativeLike> InitiativeLikes { get; set; }

    public virtual DbSet<InitiativeParticipator> InitiativeParticipators { get; set; }

    public virtual DbSet<InitiativePreset> InitiativePresets { get; set; }

    public virtual DbSet<InitiativeShare> InitiativeShares { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<Community>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("community_pkey");

            entity.ToTable("community");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<CommunityJoinRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("community_join_request_pkey");

            entity.ToTable("community_join_request");

            entity.HasIndex(e => e.CommunityId, "community_join_request_community_id_idx");

            entity.HasIndex(e => e.UserId, "community_join_request_user_id_idx");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CommunityId).HasColumnName("community_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.IsAccepted).HasColumnName("is_accepted");
            entity.Property(e => e.ReviewedBy).HasColumnName("reviewed_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Community).WithMany(p => p.CommunityJoinRequests)
                .HasForeignKey(d => d.CommunityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("community_join_request_community_id_fkey");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.CommunityJoinRequestReviewedByNavigations)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("community_join_request_reviewed_by_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.CommunityJoinRequestUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("community_join_request_user_id_fkey");
        });

        modelBuilder.Entity<CommunityResident>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.CommunityId }).HasName("community_resident_pkey");

            entity.ToTable("community_resident");

            entity.HasIndex(e => e.CommunityId, "community_resident_community_id_idx");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CommunityId).HasColumnName("community_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.IsManager).HasColumnName("is_manager");

            entity.HasOne(d => d.Community).WithMany(p => p.CommunityResidents)
                .HasForeignKey(d => d.CommunityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("community_resident_community_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.CommunityResidents)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("community_resident_user_id_fkey");
        });

        modelBuilder.Entity<DirectMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("direct_message_pkey");

            entity.ToTable("direct_message");

            entity.HasIndex(e => e.RecipientId, "direct_message_recipient_id_idx");

            entity.HasIndex(e => e.SenderId, "direct_message_sender_id_idx");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.RecipientId).HasColumnName("recipient_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");

            entity.HasOne(d => d.Recipient).WithMany(p => p.DirectMessageRecipients)
                .HasForeignKey(d => d.RecipientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("direct_message_recipient_id_fkey");

            entity.HasOne(d => d.Sender).WithMany(p => p.DirectMessageSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("direct_message_sender_id_fkey");
        });

        modelBuilder.Entity<EcoPointTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("eco_point_transaction_pkey");

            entity.ToTable("eco_point_transaction");

            entity.HasIndex(e => e.CommunityId, "eco_point_transaction_community_id_idx");

            entity.HasIndex(e => e.InitiativeId, "eco_point_transaction_initiative_id_idx");

            entity.HasIndex(e => e.UserId, "eco_point_transaction_user_id_idx");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.CommunityId).HasColumnName("community_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.InitiativeId).HasColumnName("initiative_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Community).WithMany(p => p.EcoPointTransactions)
                .HasForeignKey(d => d.CommunityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("eco_point_transaction_community_id_fkey");

            entity.HasOne(d => d.Initiative).WithMany(p => p.EcoPointTransactions)
                .HasForeignKey(d => d.InitiativeId)
                .HasConstraintName("eco_point_transaction_initiative_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.EcoPointTransactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("eco_point_transaction_user_id_fkey");
        });

        modelBuilder.Entity<Initiative>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("initiative_pkey");

            entity.ToTable("initiative");

            entity.HasIndex(e => e.CategoryId, "initiative_category_id_idx");

            entity.HasIndex(e => e.CommunityId, "initiative_community_id_idx");

            entity.HasIndex(e => e.CreatedBy, "initiative_created_by_idx");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CommunityId).HasColumnName("community_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndedAt).HasColumnName("ended_at");
            entity.Property(e => e.EstimatedEndsAt).HasColumnName("estimated_ends_at");
            entity.Property(e => e.IsPublic).HasColumnName("is_public");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PresetId).HasColumnName("preset_id");
            entity.Property(e => e.StartsAt).HasColumnName("starts_at");

            entity.HasOne(d => d.Category).WithMany(p => p.Initiatives)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("initiative_category_id_fkey");

            entity.HasOne(d => d.Community).WithMany(p => p.Initiatives)
                .HasForeignKey(d => d.CommunityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("initiative_community_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Initiatives)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("initiative_created_by_fkey");

            entity.HasOne(d => d.Preset).WithMany(p => p.Initiatives)
                .HasForeignKey(d => d.PresetId)
                .HasConstraintName("initiative_preset_id_fkey");
        });

        modelBuilder.Entity<InitiativeCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("initiative_category_pkey");

            entity.ToTable("initiative_category");

            entity.HasIndex(e => e.Name, "initiative_category_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.IconName).HasColumnName("icon_name");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<InitiativeComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("initiative_comment_pkey");

            entity.ToTable("initiative_comment");

            entity.HasIndex(e => e.InitiativeId, "initiative_comment_initiative_id_idx");

            entity.HasIndex(e => e.UserId, "initiative_comment_user_id_idx");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.InitiativeId).HasColumnName("initiative_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Initiative).WithMany(p => p.InitiativeComments)
                .HasForeignKey(d => d.InitiativeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("initiative_comment_initiative_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.InitiativeComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("initiative_comment_user_id_fkey");
        });

        modelBuilder.Entity<InitiativeCommentLike>(entity =>
        {
            entity.HasKey(e => new { e.CommentId, e.UserId }).HasName("initiative_comment_like_pkey");

            entity.ToTable("initiative_comment_like");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Comment).WithMany(p => p.InitiativeCommentLikes)
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("initiative_comment_like_comment_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.InitiativeCommentLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("initiative_comment_like_user_id_fkey");
        });

        modelBuilder.Entity<InitiativeLike>(entity =>
        {
            entity.HasKey(e => new { e.InitiativeId, e.UserId }).HasName("initiative_like_pkey");

            entity.ToTable("initiative_like");

            entity.HasIndex(e => e.UserId, "initiative_like_user_id_idx");

            entity.Property(e => e.InitiativeId).HasColumnName("initiative_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Initiative).WithMany(p => p.InitiativeLikes)
                .HasForeignKey(d => d.InitiativeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("initiative_like_initiative_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.InitiativeLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("initiative_like_user_id_fkey");
        });

        modelBuilder.Entity<InitiativeParticipator>(entity =>
        {
            entity.HasKey(e => new { e.InitiativeId, e.UserId }).HasName("initiative_participator_pkey");

            entity.ToTable("initiative_participator");

            entity.HasIndex(e => e.UserId, "initiative_participator_user_id_idx");

            entity.Property(e => e.InitiativeId).HasColumnName("initiative_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("joined_at");

            entity.HasOne(d => d.Initiative).WithMany(p => p.InitiativeParticipators)
                .HasForeignKey(d => d.InitiativeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("initiative_participator_initiative_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.InitiativeParticipators)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("initiative_participator_user_id_fkey");
        });

        modelBuilder.Entity<InitiativePreset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("initiative_preset_pkey");

            entity.ToTable("initiative_preset");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<InitiativeShare>(entity =>
        {
            entity.HasKey(e => new { e.InitiativeId, e.TargetCommunityId }).HasName("initiative_share_pkey");

            entity.ToTable("initiative_share");

            entity.HasIndex(e => e.TargetCommunityId, "initiative_share_target_community_id_idx");

            entity.Property(e => e.InitiativeId).HasColumnName("initiative_id");
            entity.Property(e => e.TargetCommunityId).HasColumnName("target_community_id");
            entity.Property(e => e.SharedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("shared_at");
            entity.Property(e => e.SharedByUserId).HasColumnName("shared_by_user_id");

            entity.HasOne(d => d.Initiative).WithMany(p => p.InitiativeShares)
                .HasForeignKey(d => d.InitiativeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("initiative_share_initiative_id_fkey");

            entity.HasOne(d => d.SharedByUser).WithMany(p => p.InitiativeShares)
                .HasForeignKey(d => d.SharedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("initiative_share_shared_by_user_id_fkey");

            entity.HasOne(d => d.TargetCommunity).WithMany(p => p.InitiativeShares)
                .HasForeignKey(d => d.TargetCommunityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("initiative_share_target_community_id_fkey");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_pkey");

            entity.ToTable("notification");

            entity.HasIndex(e => e.IsRead, "notification_is_read_idx");

            entity.HasIndex(e => e.UserId, "notification_user_id_idx");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.RefId).HasColumnName("ref_id");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notification_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "user_email_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AvatarImageUrl).HasColumnName("avatar_image_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
