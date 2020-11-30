import { LayerStyleOptions } from './layer-style-options';
import { LayerStyle } from './layer-style.enum';

export class GraduatedColorsStyleOptions implements LayerStyleOptions {
    style: LayerStyle.GraduatedColors;
    propertyName: string;
    size = 8;
    minColor = '#000000';
    maxColor = '#FFFFFF';
    minValue: number;
    maxValue: number;
}
