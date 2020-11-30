import { Component, Inject, OnInit } from '@angular/core';
import { finalize, tap } from 'rxjs/operators';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { geoJSON, GeoJSON, GeoJSONOptions } from 'leaflet';

import { EditLayerDialogParameters } from './edit-layer-dialog-parameters';
import { EditLayerDialogType } from './edit-layer-dialog-type.enum';
import { Layer } from '../../../models/layer';
import { FileService } from '../../../services/file.service';
import { getLayerTypeName, LayerType } from '../../../models/layer-type.enum';
import { getLayerStyleName, LayerStyle } from '../../../models/layer-style-options/layer-style.enum';
import { DependencyType, getDependencyTypeName } from '../../../core/dependency.type';
import { LayerStyleOptions } from '../../../models/layer-style-options/layer-style-options';
import { SimpleStyleOptions } from '../../../models/layer-style-options/simple-style-options';
import { UniqueValuesStyleOptions } from '../../../models/layer-style-options/unique-values-style-options';
import { GraduatedColorsStyleOptions } from '../../../models/layer-style-options/graduated-colors-style-options';
import { GraduatedCharactersStyleOptions } from '../../../models/layer-style-options/graduated-characters-style-options';
import { ChartDiagramStyleOptions } from '../../../models/layer-style-options/chart-diagram-style-options';
import { DensityMapStyleOptions } from '../../../models/layer-style-options/density-map-style-options';

@Component({
    selector: 'app-edit-layer-dialog',
    templateUrl: './edit-layer-dialog.component.html',
    styleUrls: [ './edit-layer-dialog.component.css' ]
})
export class EditLayerDialogComponent implements OnInit {

    public loading: boolean;

    public editLayerForm: FormGroup;
    public editLayerStyleForm: FormGroup;

    public fileName = '';

    public layerTypeOptions: LayerType[];
    public layerStyleOptions: LayerStyle[];
    public dependencyTypeOptions = [ DependencyType.Linear, DependencyType.Logarithmic ];
    public propertyNames: string[];
    public propertyValues: string[];
    public propertyNameColors: { propertyName: string, color: string }[];

    public minValueNumber: number;
    public maxValueNumber: number;

    public availableFileExtensions = '.json, .geojson, .csv, .gpx, .kml, .xslx';

    public getLayerTypeOptionName: (type: LayerType) => string;
    public getLayerStyleOptionName: (style: LayerStyle) => string;
    public getDependencyTypeOptionName: (style: DependencyType) => string;

    private valueStyleOptions: { [value: string]: SimpleStyleOptions };
    private currentPropertyValue: string;

    constructor(
        private fileService: FileService,
        private dialogRef: MatDialogRef<EditLayerDialogComponent>,
        private formBuilder: FormBuilder,
        @Inject(MAT_DIALOG_DATA)
        public data: EditLayerDialogParameters) {
    }

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

    public get layerDependency(): AbstractControl {
        return this.editLayerStyleForm.get('dependency');
    }

    public get layerFirstSize(): AbstractControl {
        return this.editLayerStyleForm.get('firstSize');
    }

    public get layerSecondSize(): AbstractControl {
        return this.editLayerStyleForm.get('secondSize');
    }

    public get layerFirstColor(): AbstractControl {
        return this.editLayerStyleForm.get('firstColor');
    }

    public get layerSecondColor(): AbstractControl {
        return this.editLayerStyleForm.get('secondColor');
    }

    public get title(): string {
        switch (this.data.type) {
            case EditLayerDialogType.Create:
                return 'Создание нового слоя';
            case EditLayerDialogType.Edit:
                return 'Редактирование слоя';
        }
    }

    public get propertyNamesForChartDiagram(): string[] {
        return this.propertyNames.filter(name => !this.propertyNameColors.map(x => x.propertyName).includes(name));
    }

    public get firstSizePlaceholder(): string {
        if (this.layerStyle.value === LayerStyle.GraduatedCharacters) {
            return 'Минимальный размер';
        }

        return 'Размер';
    }

    public get secondSizePlaceholder(): string {
        if (this.layerStyle.value === LayerStyle.GraduatedCharacters) {
            return 'Максимальный размер';
        }

        return 'Размер 2';
    }

    public get firstColorPlaceholder(): string {
        if (this.layerStyle.value === LayerStyle.GraduatedColors) {
            return 'Цвет минимального значения';
        }

        return 'Цвет обводки';
    }

    public get secondColorPlaceholder(): string {
        if (this.layerStyle.value === LayerStyle.GraduatedColors) {
            return 'Цвет максимального значения';
        }

        return 'Цвет заливки';
    }

    public get disabled(): boolean {
        return !this.editLayerForm.valid
            || !this.editLayerStyleForm.valid
            || !this.editLayerForm.dirty && !this.editLayerStyleForm.dirty;
    }

    public ngOnInit(): void {
        this.dialogRef.updateSize('420px');
        this.getLayerTypeOptionName = getLayerTypeName;
        this.getLayerStyleOptionName = getLayerStyleName;
        this.getDependencyTypeOptionName = getDependencyTypeName;
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

        this.loading = true;
        this.fileService.readFileAsGeoJson(files[0])
            .pipe(
                tap(geojson => {
                    if (geojson && !this.layerName.value?.length) {
                        this.layerName.setValue(files[0].name.slice(0, files[0].name.lastIndexOf('.')));
                    }
                    this.fileName = files[0]?.name;
                }),
                tap(geojson => this.layerData.setValue(geojson)),
                tap(() => this.layerData.markAsDirty()),
                tap(geojson => this.layerTypeOptions = this.getAvailableTypesForLayer(geojson)),
                finalize(() => this.loading = false))
            .subscribe(() => {
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
                    firstSize: simpleStyleOptions.size,
                    firstColor: simpleStyleOptions.color,
                    secondColor: simpleStyleOptions.fillColor
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
                    propertyName: [ propertyName, Validators.required ],
                    propertyValue: null,
                    firstSize: null,
                    firstColor: null,
                    secondColor: null
                });

                this.onPropertyNameChange(propertyName, style);

                break;
            }
            case LayerStyle.DensityMap: {
                const densityMapStyleOptions = this.data.currentLayer?.styleOptions.style === style
                    ? this.data.currentLayer.styleOptions as DensityMapStyleOptions
                    : new DensityMapStyleOptions();

                this.propertyNames = this.getAvailablePropertiesForLayer('number');

                const propertyName = densityMapStyleOptions?.propertyName
                    ? densityMapStyleOptions?.propertyName
                    : this.propertyNames.length > 0
                        ? this.propertyNames[0]
                        : null;

                this.editLayerStyleForm = this.formBuilder.group({
                    propertyName: [ propertyName, Validators.required ],
                    firstColor: densityMapStyleOptions.color,
                    secondColor: densityMapStyleOptions.fillColor,
                    firstSize: densityMapStyleOptions.size
                });

                this.onPropertyNameChange(propertyName, style);

                break;
            }
            case LayerStyle.GraduatedCharacters: {
                const graduatedCharactersStyleOptions = this.data.currentLayer?.styleOptions.style === style
                    ? this.data.currentLayer.styleOptions as GraduatedCharactersStyleOptions
                    : new GraduatedCharactersStyleOptions();

                this.propertyNames = this.getAvailablePropertiesForLayer('number');

                const propertyName = graduatedCharactersStyleOptions?.propertyName
                    ? graduatedCharactersStyleOptions?.propertyName
                    : this.propertyNames.length > 0
                        ? this.propertyNames[0]
                        : null;

                this.editLayerStyleForm = this.formBuilder.group({
                    propertyName: [ propertyName, Validators.required ],
                    dependency: graduatedCharactersStyleOptions.dependency,
                    firstColor: graduatedCharactersStyleOptions.color,
                    secondColor: graduatedCharactersStyleOptions.fillColor,
                    firstSize: graduatedCharactersStyleOptions.minSize,
                    secondSize: graduatedCharactersStyleOptions.maxSize,
                });

                this.onPropertyNameChange(propertyName, style);

                break;
            }
            case LayerStyle.GraduatedColors: {
                const graduatedColorsStyleOptions = this.data.currentLayer?.styleOptions.style === style
                    ? this.data.currentLayer.styleOptions as GraduatedColorsStyleOptions
                    : new GraduatedColorsStyleOptions();

                this.propertyNames = this.getAvailablePropertiesForLayer('number');

                const propertyName = graduatedColorsStyleOptions?.propertyName
                    ? graduatedColorsStyleOptions?.propertyName
                    : this.propertyNames.length > 0
                        ? this.propertyNames[0]
                        : null;

                this.editLayerStyleForm = this.formBuilder.group({
                    propertyName: [ propertyName, Validators.required ],
                    firstColor: graduatedColorsStyleOptions.minColor,
                    secondColor: graduatedColorsStyleOptions.maxColor,
                    firstSize: graduatedColorsStyleOptions.size,
                });

                this.onPropertyNameChange(propertyName, style);

                break;
            }
            case LayerStyle.ChartDiagram: {
                const chartDiagramStyleOptions = this.data.currentLayer?.styleOptions.style === style
                    ? this.data.currentLayer.styleOptions as ChartDiagramStyleOptions
                    : new ChartDiagramStyleOptions();

                this.propertyNames = this.getAvailablePropertiesForLayer('number');
                this.propertyNameColors = [];
                Object.keys(chartDiagramStyleOptions.propertyNameColors).forEach(propertyName =>
                    this.propertyNameColors.push({
                        propertyName,
                        color: chartDiagramStyleOptions.propertyNameColors[propertyName]
                    })
                );

                if (!this.propertyNameColors?.length) {
                    this.onAddAttribute();
                }

                this.editLayerStyleForm = this.formBuilder.group({
                    firstColor: chartDiagramStyleOptions.color,
                    secondColor: chartDiagramStyleOptions.fillColor,
                    firstSize: chartDiagramStyleOptions.size,
                });

                break;
            }
        }
    }

    public onPropertyNameChange(propertyName: string, style?: LayerStyle): void {
        const layerStyle = style ?? this.layerStyle.value as LayerStyle;
        switch (layerStyle) {
            case LayerStyle.UniqueValues: {
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

                break;
            }
            case LayerStyle.DensityMap:
            case LayerStyle.GraduatedCharacters:
            case LayerStyle.GraduatedColors:
                this.minValueNumber = null;
                this.maxValueNumber = null;
                this.findMinMaxValuesForLayerProperty(propertyName);
        }
    }

    public onPropertyValueChange(value: string): void {
        if (this.currentPropertyValue) {
            this.saveCurrentPropertyValue();
        }

        const simpleStyleOptions = this.valueStyleOptions[value] ?? new SimpleStyleOptions();
        this.layerFirstSize.setValue(simpleStyleOptions.size);
        this.layerFirstColor.setValue(simpleStyleOptions.color);
        this.layerSecondColor.setValue(simpleStyleOptions.fillColor);
        this.currentPropertyValue = value;
    }

    public onPropertyNameColorChange(index: number, color: string): void {
        this.propertyNameColors[index].color = color;
        this.editLayerStyleForm.markAsDirty();
    }

    public onDeleteAttribute(index: number): any {
        this.propertyNameColors.splice(index, 1);
        this.editLayerStyleForm.markAsDirty();
    }

    public onAddAttribute(): any {
        this.propertyNameColors.push({
            propertyName: this.propertyNamesForChartDiagram[0],
            color: '#000000'
        });
        this.editLayerStyleForm.markAsDirty();
    }

    private formsInit(): void {
        const style: LayerStyle = this.data.currentLayer?.styleOptions?.style ?? LayerStyle.None;
        this.editLayerForm = this.formBuilder.group({
            name: [
                this.data.currentLayer?.name,
                [ Validators.required, Validators.maxLength(64) ]
            ],
            type: [
                this.data.currentLayer?.type,
                Validators.required
            ],
            data: [ null, !!this.data.currentLayer?.data ? [] : Validators.required ],
            style
        });

        this.onStyleChange(style);
    }

    // TODO попробовать получать доступные типы при парсинге файла.
    private getAvailableTypesForLayer(data: GeoJSON.GeoJsonObject): LayerType[] {
        if (!data) {
            return [];
        }

        const availableTypes: { key: LayerType; value: boolean }[] = [
            { key: LayerType.Point, value: false },
            { key: LayerType.Line, value: false },
            { key: LayerType.Polygon, value: false }
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
                return [ ...commonStyles, LayerStyle.GraduatedCharacters ];
            case LayerType.Line:
                return commonStyles;
            case LayerType.Polygon:
                return [ ...commonStyles, LayerStyle.DensityMap ];
            case LayerType.None:
            default:
                return [];
        }
    }

    private getAvailablePropertiesForLayer(type?: string): string[] {
        const geojson: GeoJSON.GeoJsonObject = this.data.currentLayer?.data as GeoJSON.GeoJsonObject;

        switch (geojson?.type) {
            case 'FeatureCollection':
                return this.getAvailablePropertiesForFeatureCollection(geojson as GeoJSON.FeatureCollection, type);
            case 'Feature':
                return this.getAvailablePropertiesForFeature(geojson as GeoJSON.Feature, type);
            default:
                return [];
        }
    }

    private getAvailablePropertiesForFeatureCollection(featureCollection: GeoJSON.FeatureCollection, type?: string): string[] {
        let propertyNames: string[] = [];

        for (const feature of featureCollection.features) {
            propertyNames = [ ...new Set([ ...propertyNames, ...this.getAvailablePropertiesForFeature(feature, type) ]) ];
        }

        return propertyNames;
    }

    private getAvailablePropertiesForFeature(feature: GeoJSON.Feature, type?: string): string[] {
        if (!type) {
            return Object.keys(feature.properties);
        }

        const availablePropertyNames: string[] = [];
        for (const propertyName of Object.keys(feature.properties)) {
            if (typeof feature.properties[propertyName] === type || type === 'number' && !isNaN(feature.properties[propertyName])) {
                availablePropertyNames.push(propertyName);
            }
        }
        return availablePropertyNames;
    }

    private getAvailableValuesForLayerProperty(propertyName: string): string[] {
        const geojson: GeoJSON.GeoJsonObject = this.data.currentLayer?.data as GeoJSON.GeoJsonObject;

        switch (geojson?.type) {
            case 'FeatureCollection':
                return this.getAvailableValuesForFeatureCollection(geojson as GeoJSON.FeatureCollection, propertyName);
            case 'Feature':
                return [ this.getAvailableValueForFeature(geojson as GeoJSON.Feature, propertyName) ];
            default:
                return [];
        }
    }

    private getAvailableValuesForFeatureCollection(featureCollection: GeoJSON.FeatureCollection, propertyName: string): string[] {
        let values: string[] = [];

        for (const feature of featureCollection.features) {
            values = [ ...new Set([ ...values, this.getAvailableValueForFeature(feature, propertyName) ]) ];
        }

        return values;
    }

    private getAvailableValueForFeature(feature: GeoJSON.Feature, propertyName: string): string {
        const value = feature.properties[propertyName];
        return value
            ? typeof value === 'object' ? JSON.stringify(value) : value.toString()
            : 'null';
    }

    private findMinMaxValuesForLayerProperty(propertyName: string): void {
        const geojson: GeoJSON.GeoJsonObject = this.data.currentLayer?.data as GeoJSON.GeoJsonObject;

        switch (geojson?.type) {
            case 'FeatureCollection':
                for (const feature of (geojson as GeoJSON.FeatureCollection).features) {
                    this.findMinMaxValueForFeature(feature, propertyName);
                }
                break;
            case 'Feature':
                this.findMinMaxValueForFeature(geojson as GeoJSON.Feature, propertyName);
                break;
        }
    }

    private findMinMaxValueForFeature(feature: GeoJSON.Feature, propertyName: string): void {
        const value = feature.properties[propertyName];
        if (value && (typeof value === 'number' || !isNaN(value))) {
            const valueNumber = Number(value);
            this.minValueNumber = this.minValueNumber
                ? valueNumber < this.minValueNumber
                    ? valueNumber
                    : this.minValueNumber
                : valueNumber;
            this.maxValueNumber = this.maxValueNumber
                ? valueNumber > this.maxValueNumber
                    ? valueNumber
                    : this.maxValueNumber
                : valueNumber;
        }
    }

    private getLayerStyleOptions(): LayerStyleOptions {
        const style: LayerStyle = this.layerStyle.value;

        switch (style) {
            case LayerStyle.None:
                return {
                    style,
                    size: this.layerFirstSize.value,
                    color: this.layerFirstColor.value,
                    fillColor: this.layerSecondColor.value
                } as SimpleStyleOptions;
            case LayerStyle.UniqueValues:
                this.saveCurrentPropertyValue();
                return {
                    style,
                    propertyName: this.layerPropertyName.value,
                    valueStyleOptions: this.valueStyleOptions
                } as UniqueValuesStyleOptions;
            case LayerStyle.DensityMap:
                return {
                    style,
                    propertyName: this.layerPropertyName.value,
                    size: this.layerFirstSize.value,
                    color: this.layerFirstColor.value,
                    fillColor: this.layerSecondColor.value,
                    minValue: this.minValueNumber,
                    maxValue: this.maxValueNumber,
                } as DensityMapStyleOptions;
            case LayerStyle.GraduatedCharacters:
                return {
                    style,
                    propertyName: this.layerPropertyName.value,
                    minSize: this.layerFirstSize.value,
                    maxSize: this.layerSecondSize.value,
                    color: this.layerFirstColor.value,
                    fillColor: this.layerSecondColor.value,
                    minValue: this.minValueNumber,
                    maxValue: this.maxValueNumber,
                    dependency: this.layerDependency.value
                } as GraduatedCharactersStyleOptions;
            case LayerStyle.GraduatedColors:
                return {
                    style,
                    propertyName: this.layerPropertyName.value,
                    size: this.layerFirstSize.value,
                    minColor: this.layerFirstColor.value,
                    maxColor: this.layerSecondColor.value,
                    minValue: this.minValueNumber,
                    maxValue: this.maxValueNumber,
                } as GraduatedColorsStyleOptions;
            case LayerStyle.ChartDiagram:
                const propertyNameColors: { [propertyName: string]: string } = {};
                this.propertyNameColors.forEach((item) =>
                    propertyNameColors[item.propertyName] = item.color
                );
                return {
                    style,
                    size: this.layerFirstSize.value,
                    color: this.layerFirstColor.value,
                    fillColor: this.layerSecondColor.value,
                    propertyNameColors
                } as ChartDiagramStyleOptions;
        }
    }

    private saveCurrentPropertyValue(): void {
        this.valueStyleOptions[this.currentPropertyValue] = {
            style: LayerStyle.None,
            size: this.layerFirstSize.value,
            color: this.layerFirstColor.value,
            fillColor: this.layerSecondColor.value
        };
    }
}
