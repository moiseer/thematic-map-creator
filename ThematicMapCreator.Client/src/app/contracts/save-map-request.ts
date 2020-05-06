import { Layer } from '../models/layer';

export interface SaveMapRequest {
    id: string;
    name: string;
    description: string;
    userId: string;
    layers: Layer[];
}
