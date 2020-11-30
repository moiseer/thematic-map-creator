import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { finalize } from 'rxjs/operators';

import { AuthorizationService } from '../../../services/authorization.service';
import { AuthorizationContract } from '../../../contracts/authorization-contract';

@Component({
    selector: 'app-authorization-dialog',
    templateUrl: './authorization-dialog.component.html',
    styleUrls: ['./authorization-dialog.component.css']
})
export class AuthorizationDialogComponent implements OnInit {

    authorizationForm: FormGroup;
    authError: { text: string };
    loading: boolean;

    get email(): AbstractControl {
        return this.authorizationForm.controls.email;
    }

    get password(): AbstractControl {
        return this.authorizationForm.controls.password;
    }

    constructor(
        private authorizationService: AuthorizationService,
        private formBuilder: FormBuilder,
        private dialogRef: MatDialogRef<AuthorizationDialogComponent>) {
    }

    ngOnInit(): void {
        this.dialogRef.updateSize('420px');
        this.formInit();
    }

    private formInit(): void {
        this.authorizationForm = this.formBuilder.group({
            email: [
                null,
                [Validators.required, Validators.email, Validators.maxLength(64)]
            ],
            password: [
                null,
                [Validators.required, Validators.maxLength(64)]
            ]
        });
    }

    login(): void {
        const auth: AuthorizationContract = this.authorizationForm.value;
        this.loading = true;
        this.authorizationService.login(auth)
            .pipe(finalize(() => this.loading = false))
            .subscribe(
                result => {
                    if (result) {
                        this.dialogRef.close(true);
                    } else {
                        this.authError = {text: 'Неверные адреc почты и/или пароль.'};
                    }
                },
                error => {
                    this.authError = {text: error.error};
                }
            );
    }

    rememberPass() {
        alert('Для восстановления пароля свяжитесь с администратором ' +
            'по адресу электронной почты moiseew.petya@yandex.ru.');
    }
}
