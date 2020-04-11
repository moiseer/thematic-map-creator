import { Map } from '../../../models/map';

export interface MapEditDialogParameters {
    currentMap: Map;
    currentUserId: string;
    title: string;
}
