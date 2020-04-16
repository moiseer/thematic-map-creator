import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { geoJSON } from 'leaflet';

export function geoJsonStringValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        return validateGeoJson(control.value);
    };
}

export function validateGeoJson(value: string): ValidationErrors | null  {
    const error = {geojson: {valid: false}};

    try {
        const object = JSON.parse(value);
        if (object) {
            return geoJSON(object) ? null : error;
        }
        return error;
    } catch {
        return error;
    }
}
