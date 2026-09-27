using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.ResponseModel;

namespace ManagementSystem.Application.Models.Mappings;

public static class NotificationResponseMapping
{
    public static NotificationResponse ToResponse(this NotificationDto dto) =>
        new(dto.Id, dto.Type, dto.Message, dto.RelatedTaskId, dto.IsRead, dto.CreatedAt);

    public static IReadOnlyList<NotificationResponse> ToResponse(this IReadOnlyList<NotificationDto> dtos) =>
        dtos.Select(d => d.ToResponse()).ToList();
}
