import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { EditLayerDialogParameters } from './edit-layer-dialog-parameters';
import { Layer } from '../../../models/layer';
import { geoJsonStringValidator } from '../../../validators/geojson.validator';
import { FileService } from '../../../services/file.service';
import { LayerType, getLayerTypeName } from '../../../models/layer-type.enum';

@Component({
    selector: 'app-edit-layer-dialog',
    templateUrl: './edit-layer-dialog.component.html',
    styleUrls: ['./edit-layer-dialog.component.css']
})
export class EditLayerDialogComponent implements OnInit {

    get layerName(): AbstractControl {
        return this.editLayerForm.get('name');
    }

    get layerType(): AbstractControl {
        return this.editLayerForm.get('type');
    }

    get layerData(): AbstractControl {
        return this.editLayerForm.get('data');
    }

    constructor(
        private fileService: FileService,
        private dialogRef: MatDialogRef<EditLayerDialogComponent>,
        private fb: FormBuilder,
        @Inject(MAT_DIALOG_DATA)
        public data: EditLayerDialogParameters) {
    }

    public editLayerForm: FormGroup;
    public layerTypeOptions: LayerType[] = [
        LayerType.Point,
        LayerType.Line,
        LayerType.Polygon
    ];
    public getLayerTypeOptionName: (type: LayerType) => string;

    ngOnInit(): void {
        this.dialogRef.updateSize('420px');
        this.getLayerTypeOptionName = getLayerTypeName;
        this.formInit();
    }

    onSave(): void {
        const layer: Layer = {
            id: this.data.currentLayer?.id,
            index: this.data.currentLayer?.index ?? 0,
            name: this.layerName.value,
            data: this.layerData.value?.length ? JSON.parse(this.layerData.value) : this.data.currentLayer?.data,
            type: this.layerType.value,
            visible: this.data.currentLayer?.visible ?? true,
            mapId: this.data.currentMapId ?? this.data.currentLayer?.mapId
        };

        this.dialogRef.close(layer);
    }

    onFilesChange(files: FileList) {
        if (!files || !files.length) {
            return;
        }

        this.fileService.readFileAsText(files[0]).subscribe(text => {
            if (!this.layerName.value?.length) {
                this.layerName.setValue(files[0].name);
            }
            this.layerData.setValue(text);
            this.layerData.markAsDirty();
        });
    }

    private formInit(): void {
        this.editLayerForm = this.fb.group({
            name: [
                this.data.currentLayer?.name,
                [Validators.required, Validators.maxLength(64)]
            ],
            type: [
                this.data.currentLayer?.type,
                [Validators.required, Validators.min(1)]
            ],
            data: [null, geoJsonStringValidator(!!this.data.currentLayer?.data)]
        });
    }
}
