import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { Map } from '../../../models/map';
import { MapEditDialogParameters } from './map-edit-dialog-parameters';

@Component({
    selector: 'app-map-edit-dialog',
    templateUrl: './map-edit-dialog.component.html',
    styleUrls: ['./map-edit-dialog.component.css']
})
export class MapEditDialogComponent implements OnInit {

    mapEditForm: FormGroup;

    get mapName(): AbstractControl {
        return this.mapEditForm.controls.name;
    }

    get mapDescription(): AbstractControl {
        return this.mapEditForm.controls.description;
    }

    constructor(
        public dialogRef: MatDialogRef<MapEditDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: MapEditDialogParameters,
        private fb: FormBuilder) {
    }

    ngOnInit(): void {
        this.dialogRef.updateSize('420px');
        this.formInit();
    }

    formInit(): void {
        this.mapEditForm = this.fb.group({
            name: [
                this.data.currentMap?.name,
                [Validators.required, Validators.maxLength(64)]
            ],
            description: [
                this.data.currentMap?.description,
                Validators.maxLength(1024)
            ]
        });
    }

    getMap(): Map {
        // TODO получение userId и mapId для новой карты.
        return {
            id: this.data.currentMap?.id,
            name: this.mapName.value,
            description: this.mapDescription.value,
            userId: this.data.currentMap?.userId ?? this.data.currentUserId
        };
    }
}
