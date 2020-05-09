import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { geoJSON, GeoJSON, GeoJSONOptions } from 'leaflet';

import { EditLayerDialogParameters } from './edit-layer-dialog-parameters';
import { EditLayerDialogType } from './edit-layer-dialog-type.enum';
import { Layer } from '../../../models/layer';
import { FileService } from '../../../services/file.service';
import { getLayerTypeName, LayerType } from '../../../models/layer-type.enum';
import { getLayerStyleName, LayerStyle } from '../../../models/layer-style.enum';

@Component({
    selector: 'app-edit-layer-dialog',
    templateUrl: './edit-layer-dialog.component.html',
    styleUrls: ['./edit-layer-dialog.component.css']
})
export class EditLayerDialogComponent implements OnInit {

    public get layerName(): AbstractControl {
        return this.editLayerForm.get('name');
    }

    public get layerType(): AbstractControl {
        return this.editLayerForm.get('type');
    }

    public get layerData(): AbstractControl {
        return this.editLayerForm.get('data');
    }

    public get layerStyle(): AbstractControl {
        return this.editLayerForm.get('style');
    }

    public get title(): string {
        switch (this.data.type) {
            case EditLayerDialogType.Create:
                return 'Создание нового слоя';
            case EditLayerDialogType.Edit:
                return 'Редактирование слоя';
        }
    }

    constructor(
        private fileService: FileService,
        private dialogRef: MatDialogRef<EditLayerDialogComponent>,
        private formBuilder: FormBuilder,
        @Inject(MAT_DIALOG_DATA)
        public data: EditLayerDialogParameters) {
    }

    public editLayerForm: FormGroup;
    public layerTypeOptions: LayerType[];
    public layerStyleOptions: LayerStyle[];
    public availableFileExtensions = '.json, .geojson';

    public getLayerTypeOptionName: (type: LayerType) => string;
    public getLayerStyleOptionName: (style: LayerStyle) => string;

    public ngOnInit(): void {
        this.dialogRef.updateSize('420px');
        this.getLayerTypeOptionName = getLayerTypeName;
        this.getLayerStyleOptionName = getLayerStyleName;
        this.layerStyleOptions = this.getAvailableStylesForLayerType(this.data.currentLayer?.type);
        this.formInit();
    }

    public onSave(): void {
        const layer: Layer = {
            id: this.data.currentLayer?.id,
            index: this.data.currentLayer?.index ?? 0,
            name: this.layerName.value,
            data: this.layerData.value ?? this.data.currentLayer?.data,
            type: this.layerType.value,
            style: this.layerStyle.value,
            visible: this.data.currentLayer?.visible ?? true,
            mapId: this.data.currentMapId ?? this.data.currentLayer?.mapId
        };

        this.dialogRef.close(layer);
    }

    public onFilesChange(files: FileList): void {
        if (!files?.length) {
            return;
        }

        this.fileService.readFileAsGeoJson(files[0]).subscribe(geojson => {
            if (geojson && !this.layerName.value?.length) {
                this.layerName.setValue(files[0].name);
            }

            this.layerData.setValue(geojson);
            this.layerData.markAsDirty();

            this.layerTypeOptions = this.getAvailableTypesForLayer(geojson);
            if (this.layerTypeOptions.length === 1) {
                this.layerType.setValue(this.layerTypeOptions[0]);
            }
        });
    }

    private formInit(): void {
        this.editLayerForm = this.formBuilder.group({
            name: [
                this.data.currentLayer?.name,
                [Validators.required, Validators.maxLength(64)]
            ],
            type: [
                this.data.currentLayer?.type,
                Validators.required
            ],
            data: [null, !!this.data.currentLayer?.data ? [] : Validators.required],
            style: this.data.currentLayer?.style ?? LayerStyle.None
        });
    }

    // TODO попробовать получать доступные типы при парсинге файла.
    private getAvailableTypesForLayer(data: GeoJSON.GeoJsonObject): LayerType[] {
        if (!data) {
            return [];
        }

        const availableTypes: {key: LayerType; value: boolean}[] = [
            {key: LayerType.Point, value: false},
            {key: LayerType.Line, value: false},
            {key: LayerType.Polygon, value: false}
        ];

        const geoJsonOptions: GeoJSONOptions = {
            onEachFeature(feature: GeoJSON.Feature, _) {
                switch (feature.geometry.type) {
                    case 'Point':
                    case 'MultiPoint':
                        availableTypes.find(x => x.key === LayerType.Point).value = true;
                        break;
                    case 'LineString':
                    case 'MultiLineString':
                        availableTypes.find(x => x.key === LayerType.Line).value = true;
                        break;
                    case 'Polygon':
                    case 'MultiPolygon':
                        availableTypes.find(x => x.key === LayerType.Polygon).value = true;
                        break;
                }
            }
        };

        geoJSON(data, geoJsonOptions);

        return availableTypes.filter(x => x.value).map(x => x.key);
    }

    private getAvailableStylesForLayerType(type: LayerType): LayerStyle[] {
        const commonStyles: LayerStyle[] = [
            LayerStyle.None,
            LayerStyle.UniqueValues,
            LayerStyle.GraduatedColors,
            LayerStyle.ChartDiagram
        ];

        switch (type) {
            case LayerType.Point:
                return [...commonStyles, LayerStyle.GraduatedCharacters];
            case LayerType.Line:
                return commonStyles;
            case LayerType.Polygon:
                return [...commonStyles, LayerStyle.DensityMap];
            case LayerType.None:
            default:
                return [];
        }
    }
}
