import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { geoJSON } from 'leaflet';

export function geoJsonValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const error = {geojson: {valid: false}};

        try {
            const object = JSON.parse(control.value);
            if (object) {
                return geoJSON(object) ? null : error;
            }
            return error;
        } catch {
            return error;
        }
    };
}
