export class Color {
    public red: number;
    public green: number;
    public blue: number;

    public static fromHex(hex: string): Color {
        const shorthandRegex = /^#?([a-fA-F\d])([a-fA-F\d])([a-fA-F\d])$/i;
        hex = hex.replace(shorthandRegex, (m, r, g, b) => {
            return r + r + g + g + b + b;
        });

        const result = /^#?([a-fA-F\d]{2})([a-fA-F\d]{2})([a-fA-F\d]{2})$/i.exec(hex);
        return result
            ? {
                red: parseInt(result[1], 16),
                green: parseInt(result[2], 16),
                blue: parseInt(result[3], 16)
            } as Color
            : null;
    }

    public static toHex(color: Color): string {
        return '#' + [color.red, color.green, color.blue].map(x => {
            const hex = x.toString(16);
            return hex.length === 1 ? '0' + hex : hex;
        }).join('');
    }

    public static mix(minColor: Color, maxColor: Color, minValue: number, maxValue: number, value: number): Color {
        if (!(minColor && maxColor && minValue && maxValue && value)) {
            return null;
        }

        const bar = (value - minValue) / (maxValue - minValue);
        const calcColorValue = (minC: number, maxC: number): number => Math.round(bar * (maxC - minC) + minC);
        const red = calcColorValue(minColor.red, maxColor.red);
        const green = calcColorValue(minColor.green, maxColor.green);
        const blue = calcColorValue(minColor.blue, maxColor.blue);

        const color = new Color();
        color.red = red;
        color.green = green;
        color.blue = blue;

        return color;
    }

    public toHex(): string {
        return Color.toHex(this);
    }
}
