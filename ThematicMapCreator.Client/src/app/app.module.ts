import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatToolbarModule } from '@angular/material/toolbar';
import { HttpClientModule } from '@angular/common/http';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';
import { FlexLayoutModule } from '@angular/flex-layout';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MAT_SNACK_BAR_DEFAULT_OPTIONS, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MapComponent } from './components/map/map.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { SidePanelComponent } from './components/side-panel/side-panel.component';
import { MapDetailsComponent } from './components/side-panel/map-details/map-details.component';
import { EditMapDialogComponent } from './components/side-panel/edit-map-dialog/edit-map-dialog.component';
import { LayersListComponent } from './components/side-panel/layers-list/layers-list.component';
import { EditLayerDialogComponent } from './components/side-panel/edit-layer-dialog/edit-layer-dialog.component';
import { OpenMapDialogComponent } from './components/side-panel/open-map-dialog/open-map-dialog.component';
import { DeleteObjectDialogComponent } from './components/side-panel/delete-object-dialog/delete-object-dialog.component';
import { AuthorizationDialogComponent } from './components/auth/authorization-dialog/authorization-dialog.component';
import { RegistrationDialogComponent } from './components/auth/registration-dialog/registration-dialog.component';

@NgModule({
    declarations: [
        AppComponent,
        MapComponent,
        NavbarComponent,
        SidePanelComponent,
        MapDetailsComponent,
        EditMapDialogComponent,
        LayersListComponent,
        EditLayerDialogComponent,
        OpenMapDialogComponent,
        DeleteObjectDialogComponent,
        AuthorizationDialogComponent,
        RegistrationDialogComponent
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        MatToolbarModule,
        HttpClientModule,
        LeafletModule.forRoot(),
        FlexLayoutModule,
        ReactiveFormsModule,
        MatButtonModule,
        MatDialogModule,
        MatDividerModule,
        MatFormFieldModule,
        MatInputModule,
        DragDropModule,
        MatIconModule,
        MatTableModule,
        MatTooltipModule,
        MatSnackBarModule,
        MatProgressSpinnerModule,
        MatSelectModule
    ],
    providers: [{
        provide: MAT_SNACK_BAR_DEFAULT_OPTIONS,
        useValue: {duration: 3000}
    }],
    bootstrap: [AppComponent]
})
export class AppModule { }
