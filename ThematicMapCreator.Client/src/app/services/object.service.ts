import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ObjectService {

    private url = 'api/seaPorts';

    constructor(private http: HttpClient) { }

    getSeaPorts(): Observable<any[]> {
        return this.http.get<any[]>(this.url);
    }

    getVisibleSeaPorts(west: number, north: number, east: number, south: number): Observable<any[]> {
        return this.http.get<any[]>(`${this.url}/${west}/${north}/${east}/${south}`);
    }
}
