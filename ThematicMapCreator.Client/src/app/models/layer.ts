import { GeoJSON } from 'leaflet';

export interface Layer {
    id: string;
    index: number;
    name: string;
    data: GeoJSON.GeoJsonObject;
    visible: boolean;
    mapId: string;
}
