import { GeoJSON } from 'leaflet';
import { LayerType } from './layer-type.enum';
import { LayerStyleOptions } from './layer-style-options/layer-style-options';

export interface Layer {
    id: string;
    index: number;
    name: string;
    data: GeoJSON.GeoJsonObject | string;
    type: LayerType;
    styleOptions: LayerStyleOptions;
    isVisible: boolean;
    mapId: string;
}
