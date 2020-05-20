export class MathHelper {
    public static CalcProportional(minP: number, maxP: number, minValue: number, maxValue: number, value: number): number {
        const bar = (value - minValue) / (maxValue - minValue);
        return Math.round(bar * (maxP - minP) + minP);
    }
}
