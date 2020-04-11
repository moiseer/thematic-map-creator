import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { LayerEditDialogParameters } from './layer-edit-dialog-parameters';
import { Layer } from '../../../models/layer';
import {geoJSON, GeoJSON} from "leaflet";

@Component({
    selector: 'app-layer-edit-dialog',
    templateUrl: './layer-edit-dialog.component.html',
    styleUrls: ['./layer-edit-dialog.component.css']
})
export class LayerEditDialogComponent implements OnInit {

    layerEditForm: FormGroup;

    get layerName(): AbstractControl {
        return this.layerEditForm.controls.name;
    }

    get layerData(): AbstractControl {
        return this.layerEditForm.controls.data;
    }
    constructor(
        public dialogRef: MatDialogRef<LayerEditDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: LayerEditDialogParameters,
        private fb: FormBuilder) {
    }

    ngOnInit(): void {
        this.dialogRef.updateSize('420px');
        this.formInit();
    }

    formInit(): void {
        this.layerEditForm = this.fb.group({
            name: [
                this.data.currentLayer?.name,
                [Validators.required, Validators.maxLength(64)]
            ],
            data: [JSON.stringify(this.data.currentLayer?.data, null, 2)]
        });
    }

    getLayer(): Layer {
        // TODO получение layerId для нового слоя.
        return {
            id: this.data.currentLayer?.id,
            index: this.data.currentLayer?.index ?? 0,
            name: this.layerName.value,
            data: this.layerData.dirty ? JSON.parse(this.layerData.value) : this.data.currentLayer?.data,
            visible: this.data.currentLayer?.visible ?? true,
            mapId: this.data.currentLayer?.mapId ?? this.data.currentMapId,
        };
    }
}
