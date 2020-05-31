export class MathHelper {
    public static CalcProportional(minP: number,
                                   maxP: number,
                                   minValue: number,
                                   maxValue: number,
                                   value: number,
                                   round: boolean = false): number {
        const bar = (value - minValue) / (maxValue - minValue);
        const newValue = bar * (maxP - minP) + minP;
        return round ? Math.round(newValue) : newValue;
    }
}
