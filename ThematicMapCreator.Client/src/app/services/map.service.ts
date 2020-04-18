import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';

import { Map } from '../models/map';
import { Layer } from '../models/layer';
import { SaveMapLayersRequest } from '../contracts/save-map-layers-request';

@Injectable({
    providedIn: 'root'
})
export class MapService {

    private url = 'api/maps';

    public layers$: BehaviorSubject<Layer[]> = new BehaviorSubject<Layer[]>([]);

    public zoomAll$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(null);

    constructor(private http: HttpClient) {
    }

    getMaps(userId: string): Observable<Map[]> {
        // TODO return this.http.get<Map[]>(`${this.url}/user/${userId}`);
        console.log('getMaps');
        return of(this.getExampleMaps(userId));
    }

    getMap(mapId: string): Observable<Map> {
        // TODO return this.http.get<Map>(`${this.url}/${mapId}`)
        console.log('getMap');
        return of(this.getExampleMaps('1').find(map => map.id === mapId))
            .pipe(tap(map => this.getMapLayers(map.id)
                .subscribe(layers => this.layers$.next(layers))
            ));
    }

    getMapLayers(mapId: string): Observable<Layer[]> {
        // TODO return this.http.get<Layer[]>(`${this.url}/${mapId}/layers`);
        console.log('getMapLayers');
        return of(this.getExampleLayers(mapId));
    }

    saveMap(map: SaveMapLayersRequest): Observable<any> {
        // TODO return this.http.put<any>(this.url, map);
        console.log('saveMap');
        return of(null);
    }

    deleteMap(mapId: string): Observable<any> {
        // TODO return this.http.delete<any>(this.url)
        console.log('deleteMap');
        return of(null)
            .pipe(tap(() => this.layers$.next([])));
    }

    private getExampleMaps(userId: string): Map[] {
        return [
            {id: '1', name: 'New map 1', description: 'simple description', userId},
            {id: '2', name: 'New map 2', description: 'simple description', userId},
            {
                id: '3',
                name: 'Very long name Very long name Very long name Very long name',
                description: 'simple description simple description simple description',
                userId
            }
        ];
    }

    private getExampleLayers(mapId: string): Layer[] {
        return [
            {id: '1', index: 1, name: 'layer 1', visible: true, data: null, mapId},
            {id: '2', index: 2, name: 'layer 2', visible: true, data: null, mapId},
            {
                id: '3', index: 3, visible: false, data: null, mapId,
                name: 'layer 3 with very long name, layer 3 with very long name'
            },
        ];
    }
}
