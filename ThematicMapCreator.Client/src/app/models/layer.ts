import { GeoJSON } from 'leaflet';

export interface Layer {
    id: string;
    name: string;
    data: GeoJSON.GeoJsonObject;
    visible: boolean;
    mapId: string;
}
