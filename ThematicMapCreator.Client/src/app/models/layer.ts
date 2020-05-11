import { GeoJSON } from 'leaflet';
import { LayerType } from './layer-type.enum';
import { LayerStyle } from './layer-style.enum';
import { LayerStyleOptions } from './layer-style-options';

export interface Layer {
    id: string;
    index: number;
    name: string;
    data: GeoJSON.GeoJsonObject;
    type: LayerType;
    style: LayerStyle;
    styleOptions: LayerStyleOptions;
    visible: boolean;
    mapId: string;
}
