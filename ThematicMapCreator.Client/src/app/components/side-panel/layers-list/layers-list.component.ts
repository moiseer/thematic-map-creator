import { Component, Input, OnInit } from '@angular/core';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { Observable, Subscription } from 'rxjs';
import { takeWhile } from 'rxjs/operators';

import { Layer } from '../../../models/layer';
import { MapService } from '../../../services/map.service';
import { DeleteObjectDialogParameters } from '../delete-object-dialog/delete-object-dialog-parameters';
import { DeleteObjectDialogComponent } from '../delete-object-dialog/delete-object-dialog.component';
import { EditLayerDialogParameters } from '../edit-layer-dialog/edit-layer-dialog-parameters';
import { EditLayerDialogComponent } from '../edit-layer-dialog/edit-layer-dialog.component';
import { EditLayerDialogType } from '../edit-layer-dialog/edit-layer-dialog-type.enum';
import { getLayerTypeName, LayerType } from '../../../models/layer-type.enum';
import { getLayerStyleName, LayerStyle } from '../../../models/layer-style-options/layer-style.enum';

@Component({
    selector: 'app-layers-list',
    templateUrl: './layers-list.component.html',
    styleUrls: ['./layers-list.component.css'],
})
export class LayersListComponent implements OnInit {

    @Input() mapId: string;

    public layers: Layer[];

    constructor(
        private dialogService: MatDialog,
        private mapService: MapService) {
    }

    public getLayerTypeOptionName: (type: LayerType) => string;
    public getLayerStyleOptionName: (style: LayerStyle) => string;

    public ngOnInit(): void {
        this.getLayerTypeOptionName = getLayerTypeName;
        this.getLayerStyleOptionName = getLayerStyleName;
        this.subscribeToLayersChanges()
            .add(this.checkLayers());
    }

    public onDropLayer(event: CdkDragDrop<any[]>): void {
        if (event.previousIndex === event.currentIndex) {
            return;
        }
        moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
        this.reorderIndexes(this.layers);
    }

    public onZoomAll(): void {
        this.mapService.zoomAll$.next(true);
    }

    public onCreateLayer(): void {
        const dialogParams: EditLayerDialogParameters = {
            currentMapId: this.mapId,
            currentLayer: null,
            type: EditLayerDialogType.Create
        };

        this.openEditLayerDialog(dialogParams)
            .pipe(takeWhile(result => !!result))
            .subscribe(result => {
                result.index = this.layers.length;
                result.mapId = this.mapId;
                this.layers.push(result);
                this.mapService.layers$.next(this.layers);
                this.onZoomAll();
            });
    }

    public onEditLayer(layerIndex: number): void {
        const dialogParams: EditLayerDialogParameters = {
            currentMapId: this.mapId,
            currentLayer: this.layers[layerIndex],
            type: EditLayerDialogType.Edit
        };

        this.openEditLayerDialog(dialogParams)
            .pipe(takeWhile(result => !!result))
            .subscribe(result => {
                this.layers[layerIndex] = result;
                this.mapService.layers$.next(this.layers);
            });
    }

    public onDeleteLayer(layerIndex: number): void {
        const dialogParams: DeleteObjectDialogParameters = {
            objectName: `слой "${this.layers[layerIndex].name}"`
        };
        const dialogConfig: MatDialogConfig = {data: dialogParams};

        this.dialogService.open(DeleteObjectDialogComponent, dialogConfig).afterClosed()
            .pipe(takeWhile(result => result))
            .subscribe(() => {
                this.layers.splice(layerIndex, 1);
                this.reorderIndexes(this.layers);
            });
    }

    public onChangeLayerVisibility(layer: Layer): void {
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
