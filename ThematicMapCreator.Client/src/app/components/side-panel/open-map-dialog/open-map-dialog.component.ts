import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { mergeMap } from 'rxjs/operators';

import { MapService } from '../../../services/map.service';
import { UserService } from '../../../services/user.service';
import { Map } from '../../../models/map';

@Component({
    selector: 'app-open-map-dialog',
    templateUrl: './open-map-dialog.component.html',
    styleUrls: ['./open-map-dialog.component.css']
})
export class OpenMapDialogComponent implements OnInit {

    maps: Map[];
    selectedMapId: string;

    tableColumnNames: string[] = ['name'];

    constructor(
        private dialogRef: MatDialogRef<OpenMapDialogComponent>,
        private mapService: MapService,
        private userService: UserService) {
    }

    ngOnInit(): void {
        this.dialogRef.updateSize('420px');
        this.loadMaps();
    }

    onSelectMap(mapId: string): void {
        this.selectedMapId = mapId;
    }

    private loadMaps(): void {
        this.userService.getCurrentUserId()
            .pipe(mergeMap(userId => this.mapService.getMaps(userId)))
            .subscribe(maps => this.maps = maps);
    }

    isSelected(mapId: string): string {
        return mapId === this.selectedMapId ? 'selected' : '';
    }
}
