import { Component, Input } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

import { AuthorizationService } from '../../services/authorization.service';
import { User } from '../../models/user';
import { AuthorizationDialogComponent } from '../auth/authorization-dialog/authorization-dialog.component';
import { RegistrationDialogComponent } from '../auth/registration-dialog/registration-dialog.component';
import { OpenMapDialogComponent } from '../side-panel/open-map-dialog/open-map-dialog.component';
import { flatMap, takeWhile } from 'rxjs/operators';
import { MapService } from '../../services/map.service';

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
        private mapService: MapService,
        private authorizationService: AuthorizationService) {
    }

    onOpenUserDetails(): void {
        // TODO Окно юзера
    }

    onOpenMap(): void {
        this.dialogService.open(OpenMapDialogComponent).afterClosed()
            .pipe(
                takeWhile(mapId => !!mapId),
                flatMap(mapId => this.mapService.getMap(mapId)))
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
            .subscribe(() => alert('Регистрация прошла успешно.'));
    }
}
