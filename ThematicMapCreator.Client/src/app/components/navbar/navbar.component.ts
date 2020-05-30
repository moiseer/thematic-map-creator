import { Component, Input } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { finalize, flatMap, takeWhile, tap } from 'rxjs/operators';

import { AuthorizationService } from '../../services/authorization.service';
import { User } from '../../models/user';
import { AuthorizationDialogComponent } from '../auth/authorization-dialog/authorization-dialog.component';
import { RegistrationDialogComponent } from '../auth/registration-dialog/registration-dialog.component';
import { OpenMapDialogComponent } from '../dialogs/open-map-dialog/open-map-dialog.component';
import { MapService } from '../../services/map.service';

@Component({
    selector: 'app-navbar',
    templateUrl: './navbar.component.html',
    styleUrls: [ './navbar.component.css' ]
})
export class NavbarComponent {
    @Input() title: string;

    constructor(
        private snackBar: MatSnackBar,
        private dialogService: MatDialog,
        private mapService: MapService,
        private authorizationService: AuthorizationService) {
    }

    get currentUser(): User {
        return this.authorizationService.currentUser$.getValue();
    }

    onOpenUserDetails(): void {
        // TODO Окно юзера
    }

    onOpenMap(): void {
        this.dialogService.open(OpenMapDialogComponent).afterClosed()
            .pipe(
                takeWhile(mapId => !!mapId),
                tap(() => this.mapService.loading$.next(true)),
                flatMap(mapId => this.mapService.getMap(mapId)),
                finalize(() => this.mapService.loading$.next(false))
            )
            .subscribe();
    }

    logout(): void {
        this.authorizationService.logout().subscribe();
    }

    openAuthDialog(): void {
        this.dialogService.open(AuthorizationDialogComponent);
    }

    openRegDialog(): void {
        this.dialogService.open(RegistrationDialogComponent).afterClosed()
            .pipe(takeWhile(result => result))
            .subscribe(() => this.snackBar.open('Регистрация прошла успешно.'));
    }
}
