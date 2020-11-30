export enum DependencyType {
    Linear = 0,
    Logarithmic = 1
}

export function getDependencyTypeName(type: DependencyType): string {
    switch (type) {
        case DependencyType.Linear:
            return 'Линейная';
        case DependencyType.Logarithmic:
            return 'Логарифмическая';
        default:
            return 'Неизвестно';
    }
}
