import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { map, flatMap, takeWhile, tap, finalize } from 'rxjs/operators';
import { of, Subscription } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';

import { Map } from '../../../models/map';
import { EditMapDialogParameters } from '../edit-map-dialog/edit-map-dialog-parameters';
import { EditMapDialogComponent } from '../edit-map-dialog/edit-map-dialog.component';
import { MapService } from '../../../services/map.service';
import { SaveMapRequest } from '../../../contracts/save-map-request';
import { AuthorizationService } from '../../../services/authorization.service';
import { Layer } from '../../../models/layer';
import { OpenMapDialogComponent } from '../open-map-dialog/open-map-dialog.component';
import { AuthorizationDialogComponent } from '../../auth/authorization-dialog/authorization-dialog.component';

@Component({
    selector: 'app-map-details',
    templateUrl: './map-details.component.html',
    styleUrls: ['./map-details.component.css']
})
export class MapDetailsComponent implements OnInit {

    get loading(): boolean {
        return this.mapService.loading$.getValue();
    }

    get currentMap(): Map {
        return this.mapService.map$.getValue();
    }

    get currentLayers(): Layer[] {
        return this.mapService.layers$.getValue();
    }

    constructor(
        private snackBar: MatSnackBar,
        private dialogService: MatDialog,
        private mapService: MapService,
        private authorizationService: AuthorizationService) {
    }

    ngOnInit(): void {
        this.subscribeToLogout();
    }

    onEditMap(): void {
        const dialogParams: EditMapDialogParameters = {
            currentMap: this.currentMap,
            title: 'Редактирование карты'
        };

        this.openEditMapDialog(dialogParams);
    }

    onCloseMap(): void {
        this.mapService.closeMap();
    }

    onCreateMap(): void {
        const dialogParams: EditMapDialogParameters = {
            currentMap: null,
            title: 'Создание новой карты'
        };

        this.openEditMapDialog(dialogParams);
    }

    onOpenMap(): void {
        this.authorizationService.getCurrentUserId()
            .pipe(
                flatMap(userId => userId
                    ? of(userId)
                    : this.dialogService.open(AuthorizationDialogComponent).afterClosed()
                        .pipe(flatMap(() => this.authorizationService.getCurrentUserId()))
                ),
                takeWhile(userId => !!userId),
                flatMap(() => this.dialogService.open(OpenMapDialogComponent).afterClosed()),
                takeWhile(mapId => !!mapId),
                tap(() => this.mapService.loading$.next(true)),
                flatMap(mapId => this.mapService.getMap(mapId)),
                finalize(() => this.mapService.loading$.next(false))
            )
            .subscribe();
    }

    onSaveMap(): void {
        this.authorizationService.getCurrentUserId()
            .pipe(
                flatMap(userId => userId
                    ? of(userId)
                    : this.dialogService.open(AuthorizationDialogComponent).afterClosed()
                        .pipe(flatMap(() => this.authorizationService.getCurrentUserId()))
                ),
                takeWhile(userId => !!userId),
                map(userId => this.MapToSaveMapRequest(userId, this.currentMap, this.currentLayers)),
                tap(() => this.mapService.loading$.next(true)),
                flatMap(request => this.mapService.saveMap(request)),
                finalize(() => this.mapService.loading$.next(false))
            )
            .subscribe(() => this.snackBar.open('Карта сохранена'));
    }

    private openEditMapDialog(dialogParams: EditMapDialogParameters): void {
        const dialogConfig: MatDialogConfig = {data: dialogParams};

        this.dialogService.open(EditMapDialogComponent, dialogConfig).afterClosed()
            .pipe(takeWhile(result => result))
            .subscribe(result => this.mapService.map$.next(result));
    }

    private subscribeToLogout(): Subscription {
        return this.authorizationService.logout$
            .subscribe(() => this.mapService.closeMap());
    }

    private MapToSaveMapRequest(userId: string, savedMap: Map, layers: Layer[]): SaveMapRequest {
        return {
            id: savedMap.id,
            name: savedMap.name,
            settings: savedMap.settings,
            description: savedMap.description,
            userId,
            layers
        };
    }
}
