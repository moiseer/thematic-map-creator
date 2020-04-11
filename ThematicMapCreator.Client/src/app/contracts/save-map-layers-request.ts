import { Layer } from '../models/layer';

export interface SaveMapLayersRequest {
    id: string;
    name: string;
    description: string;
    userId: string;
    layers: Layer[];
}
