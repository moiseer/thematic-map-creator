import { Component, OnInit } from '@angular/core';
import {
    circleMarker, CircleMarkerOptions,
    featureGroup, FeatureGroup,
    GeoJSON, geoJSON, GeoJSONOptions,
    LatLng, latLng,
    LatLngBounds, latLngBounds,
    Layer,
    Map, MapOptions,
    PathOptions, StyleFunction,
    tileLayer
} from 'leaflet';
import { Subscription } from 'rxjs';

import { MapService } from '../../services/map.service';
import * as Models from '../../models/layer';
import { LayerType } from '../../models/layer-type.enum';
import { LayerStyle } from '../../models/layer-style-options/layer-style.enum';
import { LayerStyleOptions } from '../../models/layer-style-options/layer-style-options';
import { SimpleStyleOptions } from '../../models/layer-style-options/simple-style-options';
import { UniqueValuesStyleOptions } from '../../models/layer-style-options/unique-values-style-options';

@Component({
    selector: 'app-map',
    templateUrl: './map.component.html',
    styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit {

    private map: Map;

    layers: FeatureGroup;

    mapOptions: MapOptions = {
        zoom: 4,
        center: latLng(56.49771, 84.97437), // Tomsk.
        minZoom: 2,
        maxBounds: latLngBounds(
            [-90, -200],
            [90, 200]
        )
    };
    baseLayers: { [layerName: string]: Layer };

    constructor(private mapService: MapService) {
    }

    ngOnInit(): void {
        this.addBaseLayers();
        this.subscribeToLayersChanges();
        this.subscribeToZoomAll();
    }

    onMapReady(map: Map): void {
        this.map = map;
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
        return this.mapService.layers$.subscribe(layers =>
            this.layers = featureGroup(layers
                .filter(layer => layer.visible)
                .map(layer => geoJSON(layer.data, this.getGeoJsonOptions(layer)))
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
            onEachFeature: this.onEachFeature,
            pointToLayer: this.pointToLayer(layer.styleOptions),
            filter: this.getFilter(layer.type),
            style: this.getStyle(layer.styleOptions),
        };
    }

    private onEachFeature(feature: GeoJSON.Feature, layer: Layer): void {
        if (feature.properties) {
            const popupContent: string = JSON.stringify(feature.properties, null, 4)
                .replace(/\n*( *)},|\n*( *)],/g, () => ',')
                .replace(/\n*( *)\[|\n*( *){|}|]/g, () => '')
                .replace(/\n( *)/g, (_, p1) => '<br>' + '&nbsp;'.repeat(p1.length));
            layer.bindPopup(popupContent);
        }
    }

    private pointToLayer(styleOptions: LayerStyleOptions): (feature: GeoJSON.Feature, latlng: LatLng) => Layer {
        return (feature: GeoJSON.Feature, latlng: LatLng): Layer => {
            let simpleStyleOptions: SimpleStyleOptions;

            // TODO объеденить с методом getStyle().
            switch (styleOptions.style) {
                case LayerStyle.None: {
                    simpleStyleOptions = styleOptions as SimpleStyleOptions;
                    break;
                }
                case LayerStyle.UniqueValues: {
                    const uniqueValuesStyleOptions = styleOptions as UniqueValuesStyleOptions;

                    const propertyName = uniqueValuesStyleOptions.propertyName;
                    const value = feature.properties[propertyName];
                    const valueStr = value
                        ? typeof value === 'object' ? JSON.stringify(value) : value.toString()
                        : 'null';
                    simpleStyleOptions = uniqueValuesStyleOptions.valueStyleOptions[valueStr] ?? new SimpleStyleOptions();

                    break;
                }
                case LayerStyle.DensityMap:
                case LayerStyle.GraduatedCharacters:
                case LayerStyle.GraduatedColors:
                case LayerStyle.ChartDiagram:
                default:
                    return circleMarker(latlng);
            }

            const markerOptions: CircleMarkerOptions = {radius: simpleStyleOptions?.size};
            return circleMarker(latlng, markerOptions);
        };
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
            let simpleStyleOptions: SimpleStyleOptions;

            switch (styleOptions.style) {
                case LayerStyle.None: {
                    simpleStyleOptions = styleOptions as SimpleStyleOptions;
                    break;
                }
                case LayerStyle.UniqueValues: {
                    const uniqueValuesStyleOptions = styleOptions as UniqueValuesStyleOptions;

                    const propertyName = uniqueValuesStyleOptions.propertyName;
                    const value = feature.properties[propertyName];
                    const valueStr = value
                        ? typeof value === 'object' ? JSON.stringify(value) : value.toString()
                        : 'null';
                    simpleStyleOptions = uniqueValuesStyleOptions.valueStyleOptions[valueStr] ?? new SimpleStyleOptions();

                    break;
                }
                case LayerStyle.DensityMap:
                case LayerStyle.GraduatedCharacters:
                case LayerStyle.GraduatedColors:
                case LayerStyle.ChartDiagram:
                default:
                    return {};
            }

            return {
                color: simpleStyleOptions?.color,
                fillColor: simpleStyleOptions?.fillColor,
                weight: feature.geometry.type === 'Point' || feature.geometry.type === 'MultiPoint'
                    ? 1
                    : simpleStyleOptions?.size,
                fillOpacity: 0.5
            };
        };
    }
}
