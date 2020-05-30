import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { finalize, flatMap, mergeMap, takeWhile, tap } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

import { MapService } from '../../../services/map.service';
import { AuthorizationService } from '../../../services/authorization.service';
import { Map } from '../../../models/map';
import { DeleteObjectDialogParameters } from '../delete-object-dialog/delete-object-dialog-parameters';
import { DeleteObjectDialogComponent } from '../delete-object-dialog/delete-object-dialog.component';

@Component({
    selector: 'app-open-map-dialog',
    templateUrl: './open-map-dialog.component.html',
    styleUrls: ['./open-map-dialog.component.css']
})
export class OpenMapDialogComponent implements OnInit {

    loading: boolean;

    maps: Map[];
    selectedMapId: string;

    tableColumnNames: string[] = ['name', 'delete'];

    constructor(
        private snackBar: MatSnackBar,
        private dialogRef: MatDialogRef<OpenMapDialogComponent>,
        private dialogService: MatDialog,
        private mapService: MapService,
        private authorizationService: AuthorizationService) {
    }

    ngOnInit(): void {
        this.dialogRef.updateSize('420px');
        this.loadMaps();
    }

    onSelectMap(mapId: string): void {
        this.selectedMapId = mapId;
    }

    onDeleteMap(mapId: string): void {
        const map = this.maps.find(m => m.id === mapId);
        const dialogParams: DeleteObjectDialogParameters = {
            objectName: `карту "${map.name}"`
        };
        const dialogConfig: MatDialogConfig = {data: dialogParams};

        this.dialogService.open(DeleteObjectDialogComponent, dialogConfig).afterClosed()
            .pipe(
                takeWhile(result => result),
                flatMap(() => this.mapService.deleteMap(mapId)),
                tap(() => this.selectedMapId = null),
                tap(() => this.snackBar.open('Карта удалена'))
            )
            .subscribe(() => this.loadMaps());
    }

    private loadMaps(): void {
        this.authorizationService.getCurrentUserId()
            .pipe(
                takeWhile(userId => !!userId),
                tap(() => this.loading = true),
                mergeMap(userId => this.mapService.getMaps(userId)),
                finalize(() => this.loading = false)
            )
            .subscribe(maps => this.maps = maps);
    }
}
