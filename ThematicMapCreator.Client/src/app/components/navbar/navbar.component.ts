import { Component, Input } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

import { AuthorizationService } from '../../services/authorization.service';
import { User } from '../../models/user';
import { AuthorizationDialogComponent } from '../auth/authorization-dialog/authorization-dialog.component';
import { RegistrationDialogComponent } from '../auth/registration-dialog/registration-dialog.component';

@Component({
    selector: 'app-navbar',
    templateUrl: './navbar.component.html',
    styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
    @Input() title: string;

    get currentUser(): User {
        return this.authorizationService.currentUser$.getValue();
    }

    constructor(
        private dialogService: MatDialog,
        private authorizationService: AuthorizationService) {
    }

    onOpenUserDetails(): void {
        // TODO Окно юзера
    }

    onOpenMap(): void {
        // TODO Открытие списка карт
    }

    logout(): void {
        this.authorizationService.logout().subscribe();
    }

    openAuthDialog(): void {
        this.dialogService.open(AuthorizationDialogComponent);
    }

    openRegDialog(): void {
        // TODO Сообщение об успешной регистрации.
        this.dialogService.open(RegistrationDialogComponent);
    }
}
