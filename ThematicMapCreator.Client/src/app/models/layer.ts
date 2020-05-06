import { GeoJSON } from 'leaflet';
import { LayerOptions } from './layer-options';

export interface Layer {
    id: string;
    index: number;
    name: string;
    data: GeoJSON.GeoJsonObject;
    options: LayerOptions;
    visible: boolean;
    mapId: string;
}
