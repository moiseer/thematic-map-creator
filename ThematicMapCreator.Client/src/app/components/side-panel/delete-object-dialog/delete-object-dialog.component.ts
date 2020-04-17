import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DeleteObjectDialogParameters } from './delete-object-dialog-parameters';

@Component({
    selector: 'app-delete-object-dialog',
    templateUrl: './delete-object-dialog.component.html',
    styleUrls: ['./delete-object-dialog.component.css']
})
export class DeleteObjectDialogComponent {
    constructor(@Inject(MAT_DIALOG_DATA) public data: DeleteObjectDialogParameters) {
    }
}
