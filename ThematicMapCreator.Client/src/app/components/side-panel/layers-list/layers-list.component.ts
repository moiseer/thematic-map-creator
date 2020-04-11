import { Component, Input, OnInit } from '@angular/core';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { Observable } from 'rxjs';

import { Layer } from '../../../models/layer';
import { MapService } from '../../../services/map.service';
import { LayerEditDialogParameters } from '../layer-edit-dialog/layer-edit-dialog-parameters';
import { LayerEditDialogComponent } from '../layer-edit-dialog/layer-edit-dialog.component';

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
        this.reorderIndexes(this.layers);
    }

    onLayerCreate() {
        // TODO диалог создания слоя.
        /*const data: GeoJSON.Feature = {
            "type": "Feature",
            "geometry": {
                "type": "Point",
                "coordinates": [-104.99404, 39.75621]
            }
        };
        const layer: Layer = {id: '1', name: 'new layer', visible: true, data, mapId: '1', index: 0};
*/
        const dialogParams: LayerEditDialogParameters = {
            currentLayer: null,
            title: 'Создание нового слоя',
            currentMapId: this.mapId
        };

        this.openLayerEditDialog(dialogParams).subscribe(result => {
            if (result) {
                result.index = this.layers.length;
                this.layers.push(result);
            }
        });
    }

    onLayerEdit(layerIndex: number) {
        // TODO
        const dialogParams: LayerEditDialogParameters = {
            currentLayer: this.layers[layerIndex],
            title: 'Редактирование слоя',
            currentMapId: this.mapId
        };

        this.openLayerEditDialog(dialogParams).subscribe(result => result ? this.layers[layerIndex] = result : {});
    }

    onLayerDelete(layerIndex: number) {
        // TODO Подтверждение удаления.
        this.layers.splice(layerIndex, 1);
        this.reorderIndexes(this.layers);
    }

    onLayerVisibleChange(layer: Layer) {
        layer.visible = !layer.visible;
    }

    reorderIndexes(layers: Layer[]): void {
        layers.forEach((layer, index) => layer.index = index);
    }

    openLayerEditDialog(dialogParams: LayerEditDialogParameters): Observable<Layer> {
        const dialogConfig: MatDialogConfig = {
            data: dialogParams
        };

        const dialogRef = this.dialogService.open(LayerEditDialogComponent, dialogConfig);

        return dialogRef.afterClosed();
    }
}
