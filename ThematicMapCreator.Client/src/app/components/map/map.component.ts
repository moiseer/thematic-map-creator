import { Component, OnInit } from '@angular/core';
import {
    circleMarker, CircleMarkerOptions,
    featureGroup, FeatureGroup,
    GeoJSON, geoJSON, GeoJSONOptions,
    LatLng, latLng,
    LatLngBounds, latLngBounds,
    Layer,
    Map, MapOptions,
    PathOptions,
    tileLayer
} from 'leaflet';
import { Subscription } from 'rxjs';

import { MapService } from '../../services/map.service';
import * as Models from '../../models/layer';
import { LayerType } from '../../models/layer-type.enum';
import { LayerStyleOptions } from '../../models/layer-style-options';

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
            style: this.getStyle(layer.type, layer.styleOptions),
        };
    }

    private onEachFeature(feature: GeoJSON.Feature, layer: Layer): void {
        if (feature.properties) {
            const popupContent: string = JSON.stringify(feature.properties, null, 4)
                .replace(/\n( *)/g, (_, p1) => '<br>' + '&nbsp;'.repeat(p1.length))
                .replace(/[{}\[\]]/g, () => '');
            layer.bindPopup(popupContent);
        }
    }

    private pointToLayer(styleOptions: LayerStyleOptions): (_, latlng: LatLng) => Layer {
        const markerOptions: CircleMarkerOptions = {
            radius: styleOptions?.size
        };

        return (_, latlng: LatLng): Layer => {
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

    private getStyle(type: LayerType, styleOptions: LayerStyleOptions): PathOptions {
        return {
            color: styleOptions?.color,
            fillColor: styleOptions?.fillColor,
            weight: type === LayerType.Point ? 1 : styleOptions?.size,
            fillOpacity: 0.5
        };
    }
}
