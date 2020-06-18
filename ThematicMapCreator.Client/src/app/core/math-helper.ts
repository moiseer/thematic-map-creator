import { DependencyType } from './dependency.type';

export class MathHelper {
    public static CalcProportional(minP: number,
                                   maxP: number,
                                   minValue: number,
                                   maxValue: number,
                                   value: number,
                                   round: boolean = false,
                                   dependency: DependencyType = DependencyType.Linear): number {
        let bar: number;
        switch (dependency) {
            case DependencyType.Linear:
                bar = (value - minValue) / (maxValue - minValue);
                break;
            case DependencyType.Logarithmic:
                bar = (Math.log(value) - Math.log(minValue)) / (Math.log(maxValue) - Math.log(minValue));
        }

        const newValue = bar * (maxP - minP) + minP;
        return round ? Math.round(newValue) : newValue;
    }
}
