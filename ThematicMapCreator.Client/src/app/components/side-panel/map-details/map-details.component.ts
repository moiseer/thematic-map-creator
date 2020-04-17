import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { mergeMap, map, filter, flatMap } from 'rxjs/operators';

import { Map } from '../../../models/map';
import { EditMapDialogParameters } from '../edit-map-dialog/edit-map-dialog-parameters';
import { EditMapDialogComponent } from '../edit-map-dialog/edit-map-dialog.component';
import { MapService } from '../../../services/map.service';
import { SaveMapLayersRequest } from '../../../contracts/save-map-layers-request';
import { UserService } from '../../../services/user.service';
import { Layer } from '../../../models/layer';
import { OpenMapDialogComponent } from '../open-map-dialog/open-map-dialog.component';
import { DeleteObjectDialogParameters } from '../delete-object-dialog/delete-object-dialog-parameters';
import { DeleteObjectDialogComponent } from '../delete-object-dialog/delete-object-dialog.component';

@Component({
    selector: 'app-map-details',
    templateUrl: './map-details.component.html',
    styleUrls: ['./map-details.component.css']
})
export class MapDetailsComponent implements OnInit {

    currentMap: Map;

    get currentLayers(): Layer[] {
        return this.mapService.currentLayers;
    }

    constructor(
        private dialogService: MatDialog,
        private mapService: MapService,
        private userService: UserService) {
    }

    ngOnInit(): void {
    }

    onEditMap(): void {
        const dialogParams: EditMapDialogParameters = {
            currentMap: this.currentMap,
            title: 'Редактирование карты'
        };

        this.openEditMapDialog(dialogParams);
    }

    onDeleteMap(): void {
        const dialogParams: DeleteObjectDialogParameters = {
            objectName: `карту "${this.currentMap.name}"`
        };
        const dialogConfig: MatDialogConfig = {data: dialogParams};

        this.dialogService.open(DeleteObjectDialogComponent, dialogConfig).afterClosed()
            .pipe(
                filter(result => result),
                flatMap(() => this.mapService.deleteMap(this.currentMap.id)))
            .subscribe(() => this.currentMap = null);
    }

    onCreateMap(): void {
        const dialogParams: EditMapDialogParameters = {
            currentMap: null,
            title: 'Создание новой карты'
        };

        this.openEditMapDialog(dialogParams);
    }

    onOpenMap(): void {
        this.dialogService.open(OpenMapDialogComponent).afterClosed()
            .pipe(
                filter(mapId => mapId),
                mergeMap(mapId => this.mapService.getMap(mapId)))
            .subscribe(result => this.currentMap = result);
    }

    onSaveMap(): void {
        this.userService.getCurrentUserId()
            .pipe(
                map(userId => this.MapToSaveMapLayerRequest(userId, this.currentMap, this.currentLayers)),
                mergeMap(request => this.mapService.saveMap(request)))
            .subscribe();
    }

    private openEditMapDialog(dialogParams: EditMapDialogParameters): void {
        const dialogConfig: MatDialogConfig = { data: dialogParams };

        this.dialogService.open(EditMapDialogComponent, dialogConfig).afterClosed()
            .subscribe(result => result ? this.currentMap = result : {});
    }

    private MapToSaveMapLayerRequest(userId: string, savedMap: Map, layers: Layer[]): SaveMapLayersRequest {
        return {
            id: savedMap.id,
            name: savedMap.name,
            description: savedMap.description,
            userId,
            layers
        };
    }
}
