import { LayerStyleOptions } from './layer-style-options';
import { LayerStyle } from './layer-style.enum';

export class GraduatedColorsStyleOptions implements LayerStyleOptions {
    style = LayerStyle.GraduatedColors;
    size = 8;
    propertyName: string;
    minColor = '#000000';
    maxColor = '#FFFFFF';
    minValue: number;
    maxValue: number;
}
