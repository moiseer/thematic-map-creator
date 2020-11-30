export class SvgHelper {
    public static getPieChartHtml(data: number[], colors: string[]): string {
        if (data?.length !== colors?.length) {
            return null;
        }

        const slices: { percent: number, color: string }[] = [];
        const dataSum = data.reduce((prev, current) => current + prev);
        data.forEach((value, index) => {
            const percent = value / dataSum * 100;
            const color = colors[index];
            slices.push({ percent, color });
        });

        let svg = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32" style="
            transform: rotate(-90deg);
            border-radius: 50%;
        ">`;
        slices.forEach(slice =>
            svg = svg.concat(`<circle r="25%" cx="50%" cy="50%" style="
                stroke-dasharray: ${slice.percent} 100;
                stroke-width: 50%;
                stroke: ${slice.color};
                fill: none;
            "></circle>`)
        );

        return svg.concat(`</svg>`);
    }
}
