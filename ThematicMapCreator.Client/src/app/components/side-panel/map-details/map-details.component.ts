import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';

import { Map } from '../../../models/map';
import { MapEditDialogParameters } from '../map-edit-dialog/map-edit-dialog-parameters';
import { MapEditDialogComponent } from '../map-edit-dialog/map-edit-dialog.component';
import { MapService } from '../../../services/map.service';
import { SaveMapLayersRequest } from '../../../contracts/save-map-layers-request';

@Component({
    selector: 'app-map-details',
    templateUrl: './map-details.component.html',
    styleUrls: ['./map-details.component.css']
})
export class MapDetailsComponent implements OnInit {

    map: Map;

    constructor(
        private dialogService: MatDialog,
        private mapService: MapService) {
    }

    ngOnInit(): void {
    }

    onMapEdit(): void {
        const dialogParams: MapEditDialogParameters = {
            currentMap: this.map,
            title: 'Редактирование карты'
        };

        this.openMapEditDialog(dialogParams);
    }

    onMapDelete(): void {
        // TODO Подтверждение удаления.
        this.mapService.deleteMap(this.map.id).subscribe(() => this.map = null);
    }

    onMapCreate(): void {
        const dialogParams: MapEditDialogParameters = {
            currentMap: null,
            title: 'Создание новой карты'
        };

        this.openMapEditDialog(dialogParams);
    }

    onMapOpen(): void {
        // TODO Диалог выбора карт.
        this.mapService.getMap('1').subscribe(map => this.map = map);
    }

    onMapSave(): void {
        const request: SaveMapLayersRequest = { ...this.map, layers: this.mapService.currentLayers };

        this.mapService.saveMap(request).subscribe();
    }

    openMapEditDialog(dialogParams: MapEditDialogParameters): void {
        const dialogConfig: MatDialogConfig = {
            data: dialogParams
        };

        const dialogRef = this.dialogService.open(MapEditDialogComponent, dialogConfig);

        dialogRef.afterClosed().subscribe(result => result ? this.map = result : {});
    }
}
