import { LayerStyleOptions } from './layer-style-options';
import { SimpleStyleOptions } from './simple-style-options';
import { LayerStyle } from './layer-style.enum';

export class UniqueValuesStyleOptions implements LayerStyleOptions {
    style = LayerStyle.UniqueValues;
    propertyName: string;
    valueStyleOptions: {[value: string]: SimpleStyleOptions} = {};
}
