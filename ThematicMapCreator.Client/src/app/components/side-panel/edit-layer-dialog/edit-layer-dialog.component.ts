import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { EditLayerDialogParameters } from './edit-layer-dialog-parameters';
import { Layer } from '../../../models/layer';

@Component({
    selector: 'app-edit-layer-dialog',
    templateUrl: './edit-layer-dialog.component.html',
    styleUrls: ['./edit-layer-dialog.component.css']
})
export class EditLayerDialogComponent implements OnInit {

    editLayerForm: FormGroup;

    get layerName(): AbstractControl {
        return this.editLayerForm.controls.name;
    }

    get layerData(): AbstractControl {
        return this.editLayerForm.controls.data;
    }

    get layer(): Layer {
        return {
            id: this.data.currentLayer?.id,
            index: this.data.currentLayer?.index ?? 0,
            name: this.layerName.value,
            data: this.layerData.dirty ? JSON.parse(this.layerData.value) : this.data.currentLayer?.data,
            visible: this.data.currentLayer?.visible ?? true,
            mapId: this.data.currentLayer?.mapId
        };
    }

    constructor(
        public dialogRef: MatDialogRef<EditLayerDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: EditLayerDialogParameters,
        private fb: FormBuilder) {
    }

    ngOnInit(): void {
        this.dialogRef.updateSize('420px');
        this.formInit();
    }

    private formInit(): void {
        // TODO Валидатор для GeoJson.
        this.editLayerForm = this.fb.group({
            name: [
                this.data.currentLayer?.name,
                [Validators.required, Validators.maxLength(64)]
            ],
            data: [JSON.stringify(this.data.currentLayer?.data, null, 2)]
        });
    }
}
