import { Layer } from '../../../models/layer';

export interface EditLayerDialogParameters {
    currentMapId: string;
    currentLayer: Layer;
    title: string;
}
