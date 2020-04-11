import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { tap} from 'rxjs/operators';

import { Map } from '../models/map';
import { Layer } from '../models/layer';
import { SaveMapLayersRequest } from '../contracts/save-map-layers-request';

@Injectable({
    providedIn: 'root'
})
export class MapService {

    private url = 'api/maps';

    public currentLayers: Layer[] = [];

    constructor(private http: HttpClient) {
    }

    getMaps(userId: string): Observable<Map[]> {
        return of([{id: '1', name: 'New map', description: 'simple description', userId: '1'}]);
        // TODO return this.http.get<Map[]>(`${this.url}/user/${userId}`);
    }

    getMap(mapId: string): Observable<Map> {
        return of({id: '1', name: 'New map', description: 'simple description', userId: '1'})
        // TODO return this.http.get<Map>(`${this.url}/${mapId}`)
            .pipe(tap(map => this.getMapLayers(map.id).subscribe(layers => this.currentLayers = layers)));
    }

    getMapLayers(mapId: string): Observable<Layer[]> {
        return of([]);
        // TODO return this.http.get<Layer[]>(`${this.url}/${mapId}/layers`);
    }

    saveMap(map: SaveMapLayersRequest): Observable<any> {
        return this.http.put<any>(this.url, map);
    }

    deleteMap(mapId: string): Observable<any> {
        return of(null)
        // TODO return this.http.delete<any>(this.url)
            .pipe(tap(() => this.currentLayers = []));
    }
}
