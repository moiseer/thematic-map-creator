import { Layer } from '../../../models/layer';

export interface LayerEditDialogParameters {
    currentLayer: Layer;
    currentMapId: string;
    title: string;
}
