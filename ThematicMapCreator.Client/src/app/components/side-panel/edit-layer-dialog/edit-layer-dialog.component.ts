import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { geoJSON, GeoJSON, GeoJSONOptions } from 'leaflet';

import { EditLayerDialogParameters } from './edit-layer-dialog-parameters';
import { EditLayerDialogType } from './edit-layer-dialog-type.enum';
import { Layer } from '../../../models/layer';
import { FileService } from '../../../services/file.service';
import { getLayerTypeName, LayerType } from '../../../models/layer-type.enum';
import { getLayerStyleName, LayerStyle } from '../../../models/layer-style-options/layer-style.enum';
import { LayerStyleOptions } from '../../../models/layer-style-options/layer-style-options';
import { SimpleStyleOptions } from '../../../models/layer-style-options/simple-style-options';
import { UniqueValuesStyleOptions } from '../../../models/layer-style-options/unique-values-style-options';

@Component({
    selector: 'app-edit-layer-dialog',
    templateUrl: './edit-layer-dialog.component.html',
    styleUrls: ['./edit-layer-dialog.component.css']
})
export class EditLayerDialogComponent implements OnInit {

    private valueStyleOptions: {[value: string]: SimpleStyleOptions};
    private currentPropertyValue: string;

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

    public get layerPropertyName(): AbstractControl {
        return this.editLayerStyleForm.get('propertyName');
    }

    public get layerPropertyValue(): AbstractControl {
        return this.editLayerStyleForm.get('propertyValue');
    }

    public get layerSize(): AbstractControl {
        return this.editLayerStyleForm.get('size');
    }

    public get layerColor(): AbstractControl {
        return this.editLayerStyleForm.get('color');
    }

    public get layerFillColor(): AbstractControl {
        return this.editLayerStyleForm.get('fillColor');
    }

    public get title(): string {
        switch (this.data.type) {
            case EditLayerDialogType.Create:
                return 'Создание нового слоя';
            case EditLayerDialogType.Edit:
                return 'Редактирование слоя';
        }
    }

    public get disabled(): boolean {
        return !this.editLayerForm.valid
            || !this.editLayerStyleForm.valid
            || !this.editLayerForm.dirty && !this.editLayerStyleForm.dirty;
    }

    constructor(
        private fileService: FileService,
        private dialogRef: MatDialogRef<EditLayerDialogComponent>,
        private formBuilder: FormBuilder,
        @Inject(MAT_DIALOG_DATA)
        public data: EditLayerDialogParameters) {
    }

    public editLayerForm: FormGroup;
    public editLayerStyleForm: FormGroup;

    public layerTypeOptions: LayerType[];
    public layerStyleOptions: LayerStyle[];
    public propertyNames: string[];
    public propertyValues: string[];

    public availableFileExtensions = '.json, .geojson';

    public getLayerTypeOptionName: (type: LayerType) => string;
    public getLayerStyleOptionName: (style: LayerStyle) => string;

    public ngOnInit(): void {
        this.dialogRef.updateSize('420px');
        this.getLayerTypeOptionName = getLayerTypeName;
        this.getLayerStyleOptionName = getLayerStyleName;
        this.layerStyleOptions = this.getAvailableStylesForLayerType(this.data.currentLayer?.type);
        this.formsInit();
    }

    public onSave(): void {
        const layer: Layer = {
            id: this.data.currentLayer?.id,
            index: this.data.currentLayer?.index ?? 0,
            name: this.layerName.value,
            data: this.layerData.value ?? this.data.currentLayer?.data,
            type: this.layerType.value,
            styleOptions: this.getLayerStyleOptions(),
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

    public onStyleChange(style: LayerStyle): void {
        switch (style) {
            case LayerStyle.None: {
                const simpleStyleOptions = this.data.currentLayer?.styleOptions?.style === style
                    ? this.data.currentLayer.styleOptions as SimpleStyleOptions
                    : new SimpleStyleOptions();

                this.editLayerStyleForm = this.formBuilder.group({
                    size: simpleStyleOptions.size,
                    color: simpleStyleOptions.color,
                    fillColor: simpleStyleOptions.fillColor
                });

                break;
            }
            case LayerStyle.UniqueValues: {
                const uniqueValuesStyleOptions = this.data.currentLayer?.styleOptions.style === style
                    ? this.data.currentLayer.styleOptions as UniqueValuesStyleOptions
                    : new UniqueValuesStyleOptions();

                this.propertyNames = this.getAvailablePropertiesForLayer();

                const propertyName = uniqueValuesStyleOptions?.propertyName
                    ? uniqueValuesStyleOptions?.propertyName
                    : this.propertyNames.length > 0
                        ? this.propertyNames[0]
                        : null;

                this.editLayerStyleForm = this.formBuilder.group({
                    propertyName: [propertyName, Validators.required],
                    propertyValue: null,
                    size: null,
                    color: null,
                    fillColor: null
                });

                this.onPropertyNameChange(propertyName);

                break;
            }
            case LayerStyle.DensityMap:
            case LayerStyle.GraduatedCharacters:
            case LayerStyle.GraduatedColors:
            case LayerStyle.ChartDiagram:
            default:
                break;
        }
    }

    public onPropertyNameChange(propertyName: string): void {
        propertyName
            ? this.propertyValues = this.getAvailableValuesForLayerProperty(propertyName)
            : this.propertyValues = [];

        const uniqueValuesStyleOptions = this.data.currentLayer?.styleOptions.style === LayerStyle.UniqueValues
            ? this.data.currentLayer.styleOptions as UniqueValuesStyleOptions
            : new UniqueValuesStyleOptions();
        this.valueStyleOptions = uniqueValuesStyleOptions.valueStyleOptions;

        if (this.propertyValues.length > 0) {
            const value = this.propertyValues[0];
            this.layerPropertyValue.setValue(value);
            this.onPropertyValueChange(value);
        }
    }

    public onPropertyValueChange(value: string): void {
        if (this.currentPropertyValue) {
            this.saveCurrentPropertyValue();
        }

        const simpleStyleOptions = this.valueStyleOptions[value] ?? new SimpleStyleOptions();
        this.layerSize.setValue(simpleStyleOptions.size);
        this.layerColor.setValue(simpleStyleOptions.color);
        this.layerFillColor.setValue(simpleStyleOptions.fillColor);
        this.currentPropertyValue = value;
    }

    private formsInit(): void {
        const style: LayerStyle = this.data.currentLayer?.styleOptions?.style ?? LayerStyle.None;
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
            style
           });

        this.onStyleChange(style);
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
        // TODO доступные сейчас методы.
        return [LayerStyle.None, LayerStyle.UniqueValues];

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

    private getAvailablePropertiesForLayer(): string[] {
        const geojson: GeoJSON.GeoJsonObject = this.data.currentLayer?.data;

        switch (geojson?.type) {
            case 'FeatureCollection':
                return this.getAvailablePropertiesForFeatureCollection(geojson as GeoJSON.FeatureCollection);
            case 'Feature':
                return this.getAvailablePropertiesForFeature(geojson as GeoJSON.Feature);
            default:
                return [];
        }
    }

    private getAvailablePropertiesForFeatureCollection(featureCollection: GeoJSON.FeatureCollection): string[] {
        let propertyNames: string[] = [];

        for (const feature of featureCollection.features) {
            propertyNames = [...new Set([...propertyNames, ...this.getAvailablePropertiesForFeature(feature)])];
        }

        return propertyNames;
    }

    private getAvailablePropertiesForFeature(feature: GeoJSON.Feature): string[] {
        return Object.keys(feature.properties);
    }

    private getAvailableValuesForLayerProperty(propertyName: string): string[] {
        const geojson: GeoJSON.GeoJsonObject = this.data.currentLayer?.data;

        switch (geojson?.type) {
            case 'FeatureCollection':
                return this.getAvailableValuesForFeatureCollection(geojson as GeoJSON.FeatureCollection, propertyName);
            case 'Feature':
                return [this.getAvailableValueForFeature(geojson as GeoJSON.Feature, propertyName)];
            default:
                return [];
        }
    }

    private getAvailableValuesForFeatureCollection(featureCollection: GeoJSON.FeatureCollection, propertyName: string): string[] {
        let values: string[] = [];

        for (const feature of featureCollection.features) {
            values = [...new Set([...values, this.getAvailableValueForFeature(feature, propertyName)])];
        }

        return values;
    }

    private getAvailableValueForFeature(feature: GeoJSON.Feature, propertyName: string): string {
        const value = feature.properties[propertyName];
        return value
            ? typeof value === 'object' ? JSON.stringify(value) : value.toString()
            : 'null';
    }

    private getLayerStyleOptions(): LayerStyleOptions {
        const style: LayerStyle = this.layerStyle.value;

        switch (style) {
            case LayerStyle.None:
                return {
                    style,
                    color: this.layerColor.value,
                    fillColor: this.layerFillColor.value,
                    size: this.layerSize.value
                } as SimpleStyleOptions;
            case LayerStyle.UniqueValues:
                this.saveCurrentPropertyValue();
                return {
                    style,
                    propertyName: this.layerPropertyName.value,
                    valueStyleOptions: this.valueStyleOptions
                } as UniqueValuesStyleOptions;
            case LayerStyle.DensityMap:
            case LayerStyle.GraduatedCharacters:
            case LayerStyle.GraduatedColors:
            case LayerStyle.ChartDiagram:
                break;
        }
    }

    private saveCurrentPropertyValue(): void {
        this.valueStyleOptions[this.currentPropertyValue] = {
            style: LayerStyle.None,
            size: this.layerSize.value,
            color: this.layerColor.value,
            fillColor: this.layerFillColor.value
        };
    }
}
