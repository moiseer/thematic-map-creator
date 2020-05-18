export enum LayerStyle {
    None = 0,
    UniqueValues = 1,
    DensityMap = 2,
    GraduatedCharacters = 3,
    GraduatedColors = 4,
    ChartDiagram = 5
}

export function getLayerStyleName(style: LayerStyle): string {
    switch (style) {
        case LayerStyle.None:
            return 'Единый символ';
        case LayerStyle.UniqueValues:
            return 'Уникальные значения';
        case LayerStyle.DensityMap:
            return 'Карта плотности';
        case LayerStyle.GraduatedCharacters:
            return 'Градуированные символы';
        case LayerStyle.GraduatedColors:
            return 'Градуированные цвета';
        case LayerStyle.ChartDiagram:
            return 'Картодиаграмма';
    }
}
