import { Component, Input, OnInit } from '@angular/core';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { Observable, Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

import { Layer } from '../../../models/layer';
import { MapService } from '../../../services/map.service';
import { EditLayerDialogParameters } from '../edit-layer-dialog/edit-layer-dialog-parameters';
import { EditLayerDialogComponent } from '../edit-layer-dialog/edit-layer-dialog.component';
import { DeleteObjectDialogParameters } from '../delete-object-dialog/delete-object-dialog-parameters';
import { DeleteObjectDialogComponent } from '../delete-object-dialog/delete-object-dialog.component';

@Component({
    selector: 'app-layers-list',
    templateUrl: './layers-list.component.html',
    styleUrls: ['./layers-list.component.css'],
})
export class LayersListComponent implements OnInit {

    @Input() mapId: string;

    layers: Layer[];

    constructor(
        private dialogService: MatDialog,
        private mapService: MapService) {
    }

    ngOnInit(): void {
        this.subscribeToLayersChanges()
            .add(this.checkLayers());
    }

    onDropLayer(event: CdkDragDrop<any[]>): void {
        moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
        this.reorderIndexes(this.layers);
    }

    onZoomAll(): void {
        this.mapService.zoomAll$.next(true);
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
    onCreateLayer(): void {
        const dialogParams: EditLayerDialogParameters = {
            currentLayer: null,
            title: 'Создание нового слоя'
        };

        this.openEditLayerDialog(dialogParams).subscribe(result => {
            if (result) {
                result.index = this.layers.length;
                result.mapId = this.mapId;
                this.layers.push(result);
                this.mapService.layers$.next(this.layers);
            }
        });
    }

    onEditLayer(layerIndex: number): void {
        const dialogParams: EditLayerDialogParameters = {
            currentLayer: this.layers[layerIndex],
            title: 'Редактирование слоя'
        };

        this.openEditLayerDialog(dialogParams)
            .pipe(filter(result => !!result))
            .subscribe(result => {
                this.layers[layerIndex] = result;
                this.mapService.layers$.next(this.layers);
            });
    }

    onDeleteLayer(layerIndex: number): void {
        const dialogParams: DeleteObjectDialogParameters = {
            objectName: `слой "${this.layers[layerIndex].name}"`
        };
        const dialogConfig: MatDialogConfig = {data: dialogParams};

        this.dialogService.open(DeleteObjectDialogComponent, dialogConfig).afterClosed()
            .pipe(filter(result => result))
            .subscribe(() => {
                this.layers.splice(layerIndex, 1);
                this.reorderIndexes(this.layers);
            });
    }

    onChangeLayerVisibility(layer: Layer): void {
        layer.visible = !layer.visible;
        this.mapService.layers$.next(this.layers);
    }

    private subscribeToLayersChanges(): Subscription {
        return this.mapService.layers$.subscribe(layers => this.layers = layers);
    }

    private checkLayers(): void {
        if (this.layers.some(layer => layer.mapId !== this.mapId)) {
            this.layers = this.layers.filter(layer => layer.mapId === this.mapId);
            this.mapService.layers$.next(this.layers);
        }
    }

    private reorderIndexes(layers: Layer[]): void {
        layers.forEach((layer, index) => layer.index = index);
        this.mapService.layers$.next(this.layers);
    }

    private openEditLayerDialog(dialogParams: EditLayerDialogParameters): Observable<Layer> {
        const dialogConfig: MatDialogConfig = {data: dialogParams};

        return this.dialogService.open(EditLayerDialogComponent, dialogConfig).afterClosed();
    }
}