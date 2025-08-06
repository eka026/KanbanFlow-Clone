using AutoMapper;
using KanbanFlow.API.Dtos;
using KanbanFlow.Core.Boards;
using KanbanFlow.Core.Columns;
using KanbanFlow.Core.Tasks;

namespace KanbanFlow.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<Project, ProjectDto>();
            CreateMap<Column, ColumnDto>();
            CreateMap<TaskItem, TaskItemDto>();

            // DTO to Entity mappings
            CreateMap<CreateProjectDto, Project>();
            CreateMap<UpdateProjectDto, Project>();
            CreateMap<CreateColumnDto, Column>();
            CreateMap<UpdateColumnDto, Column>();
            CreateMap<CreateTaskItemDto, TaskItem>();
            CreateMap<UpdateTaskItemDto, TaskItem>();
        }
    }
}
