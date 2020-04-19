import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

import { AuthorizationService } from '../../../services/authorization.service';
import { RegistrationContract } from '../../../contracts/registration-contract';
import { equalsValidator } from '../../../validators/equals.validator';

@Component({
    selector: 'app-registration-dialog',
    templateUrl: './registration-dialog.component.html',
    styleUrls: ['./registration-dialog.component.css']
})
export class RegistrationDialogComponent implements OnInit {

    registrationForm: FormGroup;
    regErrorText: string;
    regError: boolean;

    get name(): AbstractControl {
        return this.registrationForm.controls.name;
    }

    get email(): AbstractControl {
        return this.registrationForm.controls.email;
    }

    get password(): AbstractControl {
        return this.registrationForm.controls.password;
    }

    get confirmPass(): AbstractControl {
        return this.registrationForm.controls.confirmPass;
    }

    constructor(
        private authorizationService: AuthorizationService,
        private formBuilder: FormBuilder,
        private dialogRef: MatDialogRef<RegistrationDialogComponent>) {
    }

    ngOnInit(): void {
        this.dialogRef.updateSize('420px');
        this.formInit();
    }

    private formInit(): void {
        this.registrationForm = this.formBuilder.group({
            name: [
                null,
                [Validators.required, Validators.maxLength(64)]
            ],
            email: [
                null,
                [Validators.required, Validators.email, Validators.maxLength(64)]
            ],
            password: [
                null,
                [Validators.required, Validators.maxLength(64)]
            ]
        });

        this.registrationForm.addControl('confirmPass',
            new FormControl(
                null,
                [Validators.required, equalsValidator(this.registrationForm, 'password')]
            )
        );
    }

    signin(): void {
        const reg: RegistrationContract = this.registrationForm.value;
        this.authorizationService.signin(reg)
            .subscribe(
                result => {
                    if (result) {
                        this.dialogRef.close(true);
                    } else {
                        this.regErrorText = '';
                        this.regError = true;
                    }
                },
                error => {
                    this.regErrorText = error.error;
                    this.regError = true;
                }
            );
    }
}
