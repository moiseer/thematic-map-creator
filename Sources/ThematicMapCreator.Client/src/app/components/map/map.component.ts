import { Component, OnInit } from '@angular/core';
import {
    circleMarker,
    CircleMarkerOptions,
    divIcon,
    DivIconOptions,
    featureGroup,
    FeatureGroup,
    GeoJSON,
    geoJSON,
    GeoJSONOptions,
    LatLng,
    latLng,
    LatLngBounds,
    latLngBounds,
    Layer,
    Map,
    MapOptions,
    Marker,
    marker,
    PathOptions,
    StyleFunction,
    tileLayer
} from 'leaflet';
import * as L from 'leaflet';
import { Subscription } from 'rxjs';
import { tap } from 'rxjs/operators';
import 'leaflet-easyprint';

import { MapService } from '../../services/map.service';
import * as Models from '../../models/layer';
import { LayerType } from '../../models/layer-type.enum';
import { LayerStyle } from '../../models/layer-style-options/layer-style.enum';
import { LayerStyleOptions } from '../../models/layer-style-options/layer-style-options';
import { SimpleStyleOptions } from '../../models/layer-style-options/simple-style-options';
import { UniqueValuesStyleOptions } from '../../models/layer-style-options/unique-values-style-options';
import { GraduatedColorsStyleOptions } from '../../models/layer-style-options/graduated-colors-style-options';
import { GraduatedCharactersStyleOptions } from '../../models/layer-style-options/graduated-characters-style-options';
import { ChartDiagramStyleOptions } from '../../models/layer-style-options/chart-diagram-style-options';
import { DensityMapStyleOptions } from '../../models/layer-style-options/density-map-style-options';
import { MathHelper } from '../../core/math-helper';
import { Color } from '../../core/color';
import { SvgHelper } from '../../core/svg-helper';

@Component({
    selector: 'app-map',
    templateUrl: './map.component.html',
    styleUrls: [ './map.component.css' ]
})
export class MapComponent implements OnInit {

    layers: FeatureGroup;
    chartLayers: Layer[];
    mapOptions: MapOptions = {
        zoom: 4,
        center: latLng(56.49771, 84.97437), // Tomsk.
        minZoom: 2,
        maxBounds: latLngBounds(
            [ -90, -200 ],
            [ 90, 200 ]
        )
    };
    baseLayers: { [layerName: string]: Layer };
    private map: Map;

    constructor(private mapService: MapService) {
    }

    ngOnInit(): void {
        this.addBaseLayers();
        this.subscribeToLayersChanges();
        this.subscribeToZoomAll();
    }

    onMapReady(map: Map): void {
        this.addEasyPrint(map);
        this.map = map;
    }

    private addEasyPrint(map: Map): void {
        (L as any).easyPrint({
            title: 'Сохранить как изображение',
            sizeModes: ['Current', 'A4Landscape', 'A4Portrait'],
            filename: `my-thematic-map`,
            exportOnly: true,
            hideControlContainer: true,
            defaultSizeTitles: {Current: 'Текущий размер', A4Landscape: 'A4 Альбомный', A4Portrait: 'A4 Портретный'}
        }).addTo(map);
    }

    private addBaseLayers(): void {
        const osm = tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
        });

        const cartoDBVoyager = tileLayer('https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> ' +
                'contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
            subdomains: 'abcd',
            maxZoom: 19
        });

        const thunderforestApiKey = 'ad0d25156c5141eb82974bc67cf4cf8f';
        const thunderforestOpenCycleMap = tileLayer(
            `https://{s}.tile.thunderforest.com/cycle/{z}/{x}/{y}.png?apikey=${thunderforestApiKey}`,
            {
                attribution: '&copy; <a href="http://www.thunderforest.com/">Thunderforest</a>, ' +
                    '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
                maxZoom: 22
            }
        );

        this.baseLayers = {
            'OpenStreetMap ': osm,
            'CartoDB Voyager': cartoDBVoyager,
            'Thunderforest OpenCycleMap': thunderforestOpenCycleMap
        };
    }

    private subscribeToLayersChanges(): Subscription {
        return this.mapService.layers$
            .pipe(tap(() => this.chartLayers = []))
            .subscribe(layers =>
                this.layers = featureGroup(layers
                    .filter(layer => layer.visible)
                    .reverse()
                    .map(layer => geoJSON(layer.data as GeoJSON.GeoJsonObject, this.getGeoJsonOptions(layer)))
                )
            );
    }

    private subscribeToZoomAll(): Subscription {
        return this.mapService.zoomAll$.subscribe(() => this.fitMapToLayers());
    }

    private fitMapToLayers(): void {
        const bounds: LatLngBounds = this.layers?.getBounds();
        if (bounds?.isValid() && this.map) {
            this.map.fitBounds(bounds);
        }
    }

    private getGeoJsonOptions(layer: Models.Layer): GeoJSONOptions {
        return {
            onEachFeature: this.onEachFeature(layer.styleOptions),
            pointToLayer: this.pointToLayer(layer.styleOptions),
            filter: this.getFilter(layer.type),
            style: this.getStyle(layer.styleOptions),
        };
    }

    private onEachFeature(styleOptions: LayerStyleOptions): (feature: GeoJSON.Feature, layer: Layer) => void {
        return (feature: GeoJSON.Feature, layer: Layer): void => {
            if (feature.properties) {
                const popupContent: string = JSON.stringify(feature.properties, null, 4)
                    .replace(/\n*( *)},|\n*( *)],/g, () => ',')
                    .replace(/\n*( *)\[|\n*( *){|}|]/g, () => '')
                    .replace(/\n( *)/g, (_, p1) => '<br>' + '&nbsp;'.repeat(p1.length));
                layer.bindPopup(popupContent);
            }

            if (styleOptions.style === LayerStyle.ChartDiagram
                && feature.geometry.type !== 'Point'
                && feature.geometry.type !== 'MultiPoint') {
                this.chartLayers.push(this.getChartMarker(styleOptions as ChartDiagramStyleOptions, feature));
            }
        };
    }

    private pointToLayer(styleOptions: LayerStyleOptions): (feature: GeoJSON.Feature, latlng: LatLng) => Layer {
        return (feature: GeoJSON.Feature, latlng: LatLng): Layer => {
            if (styleOptions.style === LayerStyle.ChartDiagram) {
                return this.getChartMarker(styleOptions as ChartDiagramStyleOptions, feature, latlng);
            }
            const simpleStyleOptions: SimpleStyleOptions = this.getSimpleStyleOption(styleOptions, feature);
            const markerOptions: CircleMarkerOptions = { radius: simpleStyleOptions?.size };
            return circleMarker(latlng, markerOptions);
        };
    }

    private getSimpleStyleOption(styleOptions: LayerStyleOptions, feature: GeoJSON.Feature): SimpleStyleOptions {
        switch (styleOptions.style) {
            case LayerStyle.None:
                return styleOptions as SimpleStyleOptions;
            case LayerStyle.UniqueValues: {
                const uniqueValuesStyleOptions = styleOptions as UniqueValuesStyleOptions;

                const propertyName = uniqueValuesStyleOptions.propertyName;
                const value = feature.properties[propertyName];
                const valueStr = value
                    ? typeof value === 'object' ? JSON.stringify(value) : value.toString()
                    : 'null';
                return uniqueValuesStyleOptions.valueStyleOptions[valueStr] ?? new SimpleStyleOptions();
            }
            case LayerStyle.DensityMap: {
                const densityMapStyleOptions = styleOptions as DensityMapStyleOptions;
                return {
                    size: densityMapStyleOptions.size,
                    color: densityMapStyleOptions.color,
                    fillColor: densityMapStyleOptions.fillColor
                } as SimpleStyleOptions;
            }
            case LayerStyle.GraduatedCharacters: {
                const graduatedCharactersStyleOptions = styleOptions as GraduatedCharactersStyleOptions;

                const propertyName = graduatedCharactersStyleOptions.propertyName;
                const value = feature.properties[propertyName];
                const valueNumber = value && (typeof value === 'number' || !isNaN(value)) ? Number(value) : 0;
                const minSize = graduatedCharactersStyleOptions.minSize;
                const maxSize = graduatedCharactersStyleOptions.maxSize;
                const minValue = graduatedCharactersStyleOptions.minValue;
                const maxValue = graduatedCharactersStyleOptions.maxValue;
                const dependency = graduatedCharactersStyleOptions.dependency;
                const size = MathHelper.CalcProportional(minSize, maxSize, minValue, maxValue, valueNumber, true, dependency);

                return size
                    ? {
                        size,
                        color: graduatedCharactersStyleOptions?.color,
                        fillColor: graduatedCharactersStyleOptions?.fillColor
                    } as SimpleStyleOptions
                    : new SimpleStyleOptions();
            }
            case LayerStyle.GraduatedColors: {
                const graduatedColorsStyleOptions = styleOptions as GraduatedColorsStyleOptions;

                const propertyName = graduatedColorsStyleOptions.propertyName;
                const value = feature.properties[propertyName];
                const valueNumber = value && (typeof value === 'number' || !isNaN(value)) ? Number(value) : 0;
                const minColor = Color.fromHex(graduatedColorsStyleOptions.minColor);
                const maxColor = Color.fromHex(graduatedColorsStyleOptions.maxColor);
                const minValue = graduatedColorsStyleOptions.minValue;
                const maxValue = graduatedColorsStyleOptions.maxValue;

                const color = Color.mix(minColor, maxColor, minValue, maxValue, valueNumber)?.toHex();
                return color
                    ? {
                        size: graduatedColorsStyleOptions?.size,
                        color,
                        fillColor: color
                    } as SimpleStyleOptions
                    : new SimpleStyleOptions();
            }
            case LayerStyle.ChartDiagram: {
                const chartDiagramStyleOptions = styleOptions as ChartDiagramStyleOptions;
                return {
                    size: chartDiagramStyleOptions.size,
                    color: chartDiagramStyleOptions.color,
                    fillColor: chartDiagramStyleOptions.fillColor
                } as SimpleStyleOptions;
            }
            default:
                return new SimpleStyleOptions();
        }
    }

    // TODO Попробовать фильтровать при выборе типа.
    private getFilter(layerType: LayerType): (feature: GeoJSON.Feature) => boolean {
        if (layerType === LayerType.None) {
            return () => true;
        }

        return (feature: GeoJSON.Feature): boolean => {
            switch (feature.geometry.type) {
                case 'Point':
                case 'MultiPoint':
                    return layerType === LayerType.Point;
                case 'LineString':
                case 'MultiLineString':
                    return layerType === LayerType.Line;
                case 'Polygon':
                case 'MultiPolygon':
                    return layerType === LayerType.Polygon;
                case 'GeometryCollection':
                    return true;
            }
        };
    }

    private getStyle(styleOptions: LayerStyleOptions): StyleFunction {
        return (feature: GeoJSON.Feature): PathOptions => {
            if ((feature.geometry.type === 'Point' || feature.geometry.type === 'MultiPoint')
                && styleOptions.style === LayerStyle.ChartDiagram) {
                return {
                    weight: 1,
                    fillOpacity: 1
                };
            }
            const simpleStyleOptions: SimpleStyleOptions = this.getSimpleStyleOption(styleOptions, feature);
            return {
                color: simpleStyleOptions?.color,
                fillColor: simpleStyleOptions?.fillColor,
                weight: feature.geometry.type === 'Point' || feature.geometry.type === 'MultiPoint'
                    ? 1
                    : simpleStyleOptions?.size,
                fillOpacity: styleOptions.style === LayerStyle.DensityMap
                    ? this.getFillOpacity(styleOptions as DensityMapStyleOptions, feature)
                    : 1
            };
        };
    }

    private getChartMarker(styleOptions: ChartDiagramStyleOptions, feature: GeoJSON.Feature, latlng?: LatLng): Marker {
        const data: number[] = [];
        const colors: string[] = [];

        Object.keys(styleOptions.propertyNameColors).forEach((propertyName, index) => {
            const value = feature.properties[propertyName];
            const valueNumber = value && (typeof value === 'number' || !isNaN(value)) ? Number(value) : 0;
            const color = styleOptions.propertyNameColors[propertyName] ?? '#000000';

            data.push(valueNumber);
            colors.push(color);
        });

        latlng = latlng ?? this.getChartMarkerLatLng(feature);
        const size = styleOptions.size;
        const iconOptions: DivIconOptions = {
            iconSize: [ size, size ],
            iconAnchor: [ size / 2, size / 2 ],
            html: SvgHelper.getPieChartHtml(data, colors),
            className: 'svg-icon',
        };

        return marker(latlng, { icon: divIcon(iconOptions) });
    }

    private getChartMarkerLatLng(feature: GeoJSON.Feature): LatLng {
        return geoJSON(feature).getBounds().getCenter();
    }

    private getFillOpacity(styleOptions: DensityMapStyleOptions, feature: GeoJSON.Feature): number {
        const propertyName = styleOptions.propertyName;
        const value = feature.properties[propertyName];
        const valueNumber = value && (typeof value === 'number' || !isNaN(value)) ? Number(value) : 0;
        const minValue = styleOptions.minValue;
        const maxValue = styleOptions.maxValue;

        return MathHelper.CalcProportional(0, 1, minValue, maxValue, valueNumber);
    }
}
