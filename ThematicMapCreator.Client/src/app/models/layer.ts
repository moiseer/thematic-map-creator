import { GeoJSON } from 'leaflet';
import { LayerType } from './layer-type.enum';
import { LayerStyle } from './layer-style.enum';

export interface Layer {
    id: string;
    index: number;
    name: string;
    data: GeoJSON.GeoJsonObject;
    type: LayerType;
    style: LayerStyle;
    visible: boolean;
    mapId: string;
}
