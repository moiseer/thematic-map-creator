import { Component, Input, OnInit } from '@angular/core';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { Observable } from 'rxjs';

import { Layer } from '../../../models/layer';
import { MapService } from '../../../services/map.service';
import { EditLayerDialogParameters } from '../edit-layer-dialog/edit-layer-dialog-parameters';
import { EditLayerDialogComponent } from '../edit-layer-dialog/edit-layer-dialog.component';

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

    onDropLayer(event: CdkDragDrop<any[]>) {
        moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
        this.reorderIndexes(this.layers);
    }

    /* Example GeoJson:
    {
        "type": "Feature",
        "geometry": {
            "type": "Point",
            "coordinates": [-104.99404, 39.75621]
        }
    }
    */
    onCreateLayer() {
        const dialogParams: EditLayerDialogParameters = {
            currentLayer: null,
            title: 'Создание нового слоя'
        };

        this.openEditLayerDialog(dialogParams).subscribe(result => {
            if (result) {
                result.index = this.layers.length;
                result.mapId = this.mapId;
                this.layers.push(result);
            }
        });
    }

    onEditLayer(layerIndex: number) {
        const dialogParams: EditLayerDialogParameters = {
            currentLayer: this.layers[layerIndex],
            title: 'Редактирование слоя'
        };

        this.openEditLayerDialog(dialogParams).subscribe(result => result ? this.layers[layerIndex] = result : {});
    }

    onDeleteLayer(layerIndex: number) {
        // TODO Подтверждение удаления.
        this.layers.splice(layerIndex, 1);
        this.reorderIndexes(this.layers);
    }

    onChangeLayerVisibility(layer: Layer) {
        layer.visible = !layer.visible;
    }

    private checkLayers(): void {
        if (this.layers.some(layer => layer.mapId !== this.mapId)) {
            this.layers = this.layers.filter(layer => layer.mapId === this.mapId);
        }
    }

    private reorderIndexes(layers: Layer[]): void {
        layers.forEach((layer, index) => layer.index = index);
    }

    private openEditLayerDialog(dialogParams: EditLayerDialogParameters): Observable<Layer> {
        const dialogConfig: MatDialogConfig = {data: dialogParams};
        const dialogRef = this.dialogService.open(EditLayerDialogComponent, dialogConfig);

        return dialogRef.afterClosed();
    }
}
