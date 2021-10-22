using ThematicMapCreator.Contracts;
using ThematicMapCreator.Domain.Models;

namespace ThematicMapCreator.Host.Extensions
{
    public static class MappingExtensions
    {
        public static MapDto ToDto(this Map map) => new()
        {
            Id = map.Id,
            Name = map.Name,
            Description = map.Description,
            UserId = map.UserId
        };

        public static UserDto ToDto(this User user) => new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };

        public static LayerDto ToDto(this Layer layer) => new()
        {
            Id = layer.Id,
            Name = layer.Name,
            Description = layer.Description,
            Index = layer.Index,
            IsVisible = layer.IsVisible,
            MapId = layer.MapId,
            Data = layer.Data,
            StyleOptions = layer.StyleOptions,
            Type = (int)layer.Type
        };
    }
}
