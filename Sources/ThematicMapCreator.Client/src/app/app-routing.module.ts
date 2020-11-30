import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SidePanelComponent } from './components/side-panel/side-panel.component';


const routes: Routes = [
    { path: '', component: SidePanelComponent },
    { path: 'maps', redirectTo: '' },
    { path: 'maps/:id', component: SidePanelComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
