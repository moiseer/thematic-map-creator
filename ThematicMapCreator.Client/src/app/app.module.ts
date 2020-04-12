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
        OpenMapDialogComponent
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
        MatTableModule
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
