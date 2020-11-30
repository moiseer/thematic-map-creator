import { AbstractControl, FormGroup, ValidationErrors, ValidatorFn } from '@angular/forms';

export function equalsValidator(form: FormGroup, firstControlName: string): ValidatorFn {
    return (secondControl: AbstractControl): ValidationErrors | null => {
        const first = form?.get(firstControlName)?.value;
        const second = secondControl?.value;

        return first === second ? null : {equals: {valid: false, first, second}};
    };
}
