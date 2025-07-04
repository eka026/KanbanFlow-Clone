using AutoMapper;
using KanbanFlow.API.Dtos;
using KanbanFlow.Core;

namespace KanbanFlow.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity to DTO
        CreateMap<Project, ProjectDto>();
        CreateMap<Column, ColumnDto>();
        CreateMap<TaskItem, TaskItemDto>();

        // DTO to Entity
        CreateMap<CreateProjectDto, Project>();
        CreateMap<UpdateProjectDto, Project>();
        CreateMap<CreateColumnDto, Column>();
        CreateMap<UpdateColumnDto, Column>();
        CreateMap<CreateTaskItemDto, TaskItem>();
        CreateMap<UpdateTaskItemDto, TaskItem>();
    }
}
