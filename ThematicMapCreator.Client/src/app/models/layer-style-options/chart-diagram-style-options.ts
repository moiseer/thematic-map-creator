import { LayerStyleOptions } from './layer-style-options';
import { LayerStyle } from './layer-style.enum';

export class ChartDiagramStyleOptions implements LayerStyleOptions {
    style: LayerStyle.ChartDiagram;
    size = 8;
    color = '#000000';
    fillColor = '#000000';
    propertyNameColors: { [propertyName: string]: string } = {};
}
