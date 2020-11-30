export enum LayerType {
    None = 0,
    Point = 1,
    Line = 2,
    Polygon = 4
}

export function getLayerTypeName(type: LayerType): string {
    switch (type) {
        case LayerType.Point:
            return 'Точечный';
        case LayerType.Line:
            return 'Линейный';
        case LayerType.Polygon:
            return 'Полигональный';
        case LayerType.None:
        default:
            return 'Неизвестно';
    }
}
