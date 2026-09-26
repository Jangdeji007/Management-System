using ManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagementSystem.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("TaskItems");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(4000);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.HasIndex(t => new { t.AssigneeId, t.Status });

        builder.HasIndex(t => t.DueDate);

        builder.HasIndex(t => t.Priority);

        builder.HasOne(t => t.Assignee)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.CreatedBy)
            .WithMany(u => u.CreatedTasks)
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Team)
            .WithMany(team => team.Tasks)
            .HasForeignKey(t => t.TeamId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
