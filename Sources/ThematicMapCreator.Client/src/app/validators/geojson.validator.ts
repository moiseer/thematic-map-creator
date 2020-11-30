import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { geoJSON } from 'leaflet';

export function geoJsonStringValidator(nullable: boolean = false): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const error = {geojson: {valid: false}};

        try {
            const object = JSON.parse(control.value);
            return object
                ? geoJSON(object) ? null : error
                : nullable ? null : error;
        } catch {
            return error;
        }
    };
}
