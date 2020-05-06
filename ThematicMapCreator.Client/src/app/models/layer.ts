import { GeoJSON } from 'leaflet';
import { LayerType } from './layer-type.enum';

export interface Layer {
    id: string;
    index: number;
    name: string;
    data: GeoJSON.GeoJsonObject;
    type: LayerType;
    visible: boolean;
    mapId: string;
}
