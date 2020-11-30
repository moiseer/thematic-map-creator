import { Layer } from '../../../models/layer';
import { EditLayerDialogType } from './edit-layer-dialog-type.enum';

export interface EditLayerDialogParameters {
    currentMapId: string;
    currentLayer: Layer;
    type: EditLayerDialogType;
}
