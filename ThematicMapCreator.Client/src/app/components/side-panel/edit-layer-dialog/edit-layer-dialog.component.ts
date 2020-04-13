import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { EditLayerDialogParameters } from './edit-layer-dialog-parameters';
import { Layer } from '../../../models/layer';
import { geoJsonValidator } from '../../../validators/geojson.validator';

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

    constructor(
        private dialogRef: MatDialogRef<EditLayerDialogComponent>,
        private fb: FormBuilder,
        @Inject(MAT_DIALOG_DATA) public data: EditLayerDialogParameters) {
    }

    ngOnInit(): void {
        this.dialogRef.updateSize('420px');
        this.formInit();
    }

    onSave(): void {
        const layer: Layer = {
            id: this.data.currentLayer?.id,
            index: this.data.currentLayer?.index ?? 0,
            name: this.layerName.value,
            data: this.layerData.dirty ? JSON.parse(this.layerData.value) : this.data.currentLayer?.data,
            visible: this.data.currentLayer?.visible ?? true,
            mapId: this.data.currentLayer?.mapId
        };

        this.dialogRef.close(layer);
    }

    private formInit(): void {
        // TODO Валидатор для GeoJson.
        this.editLayerForm = this.fb.group({
            name: [
                this.data.currentLayer?.name,
                [Validators.required, Validators.maxLength(64)]
            ],
            data: [
                JSON.stringify(this.data.currentLayer?.data, null, 2),
                geoJsonValidator()
            ]
        });
    }
}
