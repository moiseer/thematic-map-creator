import { OnInit, Component } from '@angular/core';
import {
    featureGroup, FeatureGroup,
    geoJSON, GeoJSONOptions,
    icon,
    LatLng, latLng,
    LatLngBounds, latLngBounds,
    Layer,
    Map, MapOptions,
    marker, MarkerOptions,
    tileLayer
} from 'leaflet';

import { MapService } from '../../services/map.service';

@Component({
    selector: 'app-map',
    templateUrl: './map.component.html',
    styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit {

    private map: Map;

    mapOptions: MapOptions = {
        zoom: 4,
        center: latLng(56.49771, 84.97437), // Tomsk.
        minZoom: 2,
        maxBounds: latLngBounds(
            [-90, -200],
            [90, 200]
        )
    };
    baseLayers: {[layerName: string]: Layer};

    geoJsonOptions: GeoJSONOptions = {
        onEachFeature: this.onEachFeature,
        pointToLayer: this.pointToLayer
    };

    get layers(): FeatureGroup {
        console.log('get layers');
        return featureGroup(this.mapService.currentLayers
            .filter(layer => layer.visible)
            .map(layer => geoJSON(layer.data, this.geoJsonOptions)));
    }

    constructor(private mapService: MapService) {
    }

    ngOnInit(): void {
        this.addBaseLayers();
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

    private subscribeToZoomAll(): void {
        this.mapService.zoomAll$.subscribe(() => this.fitMapToLayers());
    }

    private fitMapToLayers(): void {
        const bounds: LatLngBounds = this.layers.getBounds();
        if (bounds.isValid() && this.map) {
            this.map.fitBounds(bounds);
        }
    }

    private onEachFeature(feature, layer) {
        if (feature.properties) {
            const popupContent: string = JSON.stringify(feature.properties, null, 4)
                .replace(/\n( *)/g, (match, p1) => '<br>' + '&nbsp;'.repeat(p1.length))
                .replace(/[{}]/g, () => '');
            layer.bindPopup(popupContent);
        }
    }

    private pointToLayer(_, latlng: LatLng): Layer {
        const markerOptions: MarkerOptions = {
            icon: icon({
                iconSize: [25, 41],
                iconAnchor: [13, 41],
                iconUrl: 'leaflet/marker-icon.png',
                shadowUrl: 'leaflet/marker-shadow.png'
            })
        };
        return marker(latlng, markerOptions);
    }
}
