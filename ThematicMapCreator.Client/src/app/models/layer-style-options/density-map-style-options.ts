import { LayerStyleOptions } from './layer-style-options';
import { LayerStyle } from './layer-style.enum';

export class DensityMapStyleOptions implements LayerStyleOptions {
    style: LayerStyle.DensityMap;
    size = 8;
    color = '#000000';
    fillColor = '#000000';
    propertyName: string;
    minValue: number;
    maxValue: number;
}
