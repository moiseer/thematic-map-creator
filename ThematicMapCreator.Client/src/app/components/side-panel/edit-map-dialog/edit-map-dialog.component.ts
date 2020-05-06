import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { Map } from '../../../models/map';
import { EditMapDialogParameters } from './edit-map-dialog-parameters';

@Component({
    selector: 'app-edit-map-dialog',
    templateUrl: './edit-map-dialog.component.html',
    styleUrls: ['./edit-map-dialog.component.css']
})
export class EditMapDialogComponent implements OnInit {

    editMapForm: FormGroup;

    get mapName(): AbstractControl {
        return this.editMapForm.controls.name;
    }

    get mapDescription(): AbstractControl {
        return this.editMapForm.controls.description;
    }

    constructor(
        private dialogRef: MatDialogRef<EditMapDialogComponent>,
        private fb: FormBuilder,
        @Inject(MAT_DIALOG_DATA) public data: EditMapDialogParameters) {
    }

    ngOnInit(): void {
        this.dialogRef.updateSize('420px');
        this.formInit();
    }

    onSave(): void {
        const map: Map = {
            id: this.data.currentMap?.id,
            name: this.mapName.value,
            description: this.mapDescription.value,
            userId: this.data.currentMap?.userId
        };

        this.dialogRef.close(map);
    }

    private formInit(): void {
        this.editMapForm = this.fb.group({
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

}
