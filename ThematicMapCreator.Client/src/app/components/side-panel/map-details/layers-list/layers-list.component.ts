import {Component, Input, OnInit} from '@angular/core';
import {CdkDragDrop, moveItemInArray} from '@angular/cdk/drag-drop';
import {MatDialog} from '@angular/material/dialog';

import {Layer} from '../../../../models/layer';
import {MapService} from '../../../../services/map.service';
import {GeoJSON} from 'leaflet';

@Component({
    selector: 'app-layers-list',
    templateUrl: './layers-list.component.html',
    styleUrls: ['./layers-list.component.css'],
})
export class LayersListComponent implements OnInit {

    @Input() mapId: string;

    get layers(): Layer[] {
        return this.mapService.currentLayers;
    }

    set layers(layers: Layer[]) {
        this.mapService.currentLayers = layers;
    }

    constructor(
        private dialogService: MatDialog,
        private mapService: MapService) {
    }

    ngOnInit(): void {
        this.checkLayers();
    }

    checkLayers(): void {
        if (this.layers.some(layer => layer.mapId !== this.mapId)) {
            this.layers = this.layers.filter(layer => layer.mapId === this.mapId);
        }
    }

    drop(event: CdkDragDrop<any[]>) {
        moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    }

    onLayerCreate() {
        // TODO диалог создания слоя.
        const data: GeoJSON.Feature = {
            type: 'Feature',
            properties: {
                name: 'Coors Field',
                amenity: 'Baseball Stadium',
                popupContent: 'This is where the Rockies play!'
            },
            geometry: {
                type: 'Point',
                coordinates: [-104.99404, 39.75621]
            }
        };
        this.layers.push({id: '1', name: 'new layer', visible: true, data, mapId: '1'});
    }

    onLayerEdit(layer: Layer) {
        // TODO
    }

    onMapDelete(layerIndex: number) {
        // TODO Подтверждение удаления.
        this.layers.splice(layerIndex, 1);
    }

    onLayerVisibleChange(layer: Layer) {
        layer.visible = !layer.visible;
    }
}
