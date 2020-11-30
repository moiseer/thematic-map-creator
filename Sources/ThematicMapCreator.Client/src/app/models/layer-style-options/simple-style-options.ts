import { LayerStyleOptions } from './layer-style-options';
import { LayerStyle } from './layer-style.enum';

export class SimpleStyleOptions implements LayerStyleOptions {
    style: LayerStyle.None;
    size = 8;
    color = '#000000';
    fillColor = '#000000';
}
