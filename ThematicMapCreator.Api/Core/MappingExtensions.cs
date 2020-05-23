using System;
using System.Threading.Tasks;
using BAMCIS.GeoJSON;
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
            JsonConverter converter = applicationBuilder.ApplicationServices.GetRequiredService<KeyJsonConverter<ILayerStyleOptions, LayerStyle>>();

            TypeAdapterConfig<Layer, LayerOverview>.NewConfig()
                .Map(dest => dest.Data, source => JsonConvert.DeserializeObject<GeoJson>(source.Data))
                .Map(dest => dest.StyleOptions, source => JsonConvert.DeserializeObject<ILayerStyleOptions>(source.StyleOptions, converter));
            TypeAdapterConfig<LayerOverview, Layer>.NewConfig()
                .Map(dest => dest.Data, source => JsonConvert.SerializeObject(source.Data))
                .Map(dest => dest.StyleOptions, source => JsonConvert.SerializeObject(source.StyleOptions));

            TypeAdapterConfig<SaveMapRequest, Map>.NewConfig()
                .Map(dest => dest.Layers, source => null as object);

            return applicationBuilder;
        }

        public static async Task<TDestination> AdaptAsync<TSource, TDestination>(this Task<TSource> source) => (await source).Adapt<TDestination>();
    }
}
