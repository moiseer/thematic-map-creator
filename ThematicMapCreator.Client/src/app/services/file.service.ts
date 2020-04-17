import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class FileService {

    constructor() {
    }

    readFileAsText(file: File): Observable<string> {
        const result = new Subject<string>();
        const reader = new FileReader();
        reader.onload = () => {
            result.next(reader.result as string);
        };

        reader.readAsText(file);

        return result.asObservable();
    }
}
