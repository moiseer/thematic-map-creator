using System;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ThematicMapCreator.Api.Contracts;
using ThematicMapCreator.Api.Contracts.LayerStyleOptions;
using ThematicMapCreator.Api.Models;

namespace ThematicMapCreator.Api.Core
{
    public static class MappingExtensions
    {
        public static IApplicationBuilder UseMapping(this IApplicationBuilder applicationBuilder)
        {
            using var scope = applicationBuilder.ApplicationServices.CreateScope();
            JsonConverter converter = scope.ServiceProvider.GetRequiredService<KeyJsonConverter<ILayerStyleOptions, LayerStyle>>();

            TypeAdapterConfig<Layer, LayerOverview>.NewConfig()
                .Map(dest => dest.StyleOptions, source => JsonConvert.DeserializeObject<ILayerStyleOptions>(source.StyleOptions, converter));
            TypeAdapterConfig<LayerOverview, Layer>.NewConfig()
                .Map(dest => dest.StyleOptions, source => JsonConvert.SerializeObject(source.StyleOptions));

            TypeAdapterConfig<SaveMapRequest, Map>.NewConfig()
                .Map(dest => dest.Layers, source => null as object);

            TypeAdapterConfig<RegistrationRequest, User>.NewConfig()
                .Map(dest => dest.Id, () => Guid.NewGuid())
                .Map(dest => dest.PasswordHash, source => source.Password.GetSha256());

            return applicationBuilder;
        }

        public static async Task<TDestination> AdaptAsync<TSource, TDestination>(this Task<TSource> source) => (await source).Adapt<TDestination>();
    }
}
