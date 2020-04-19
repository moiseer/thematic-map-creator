import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { flatMap, tap } from 'rxjs/operators';

import { Map } from '../models/map';
import { Layer } from '../models/layer';
import { SaveMapRequest } from '../contracts/save-map-request';

@Injectable({
    providedIn: 'root'
})
export class MapService {

    private url = 'https://localhost:5001/api/maps';

    public map$: BehaviorSubject<Map> = new BehaviorSubject<Map>(null);
    public layers$: BehaviorSubject<Layer[]> = new BehaviorSubject<Layer[]>([]);

    public zoomAll$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(null);

    constructor(private http: HttpClient) {
    }

    getMaps(userId: string): Observable<Map[]> {
        // console.log('getMaps');
        // return of(this.getExampleMaps(userId));
        return this.http.get<Map[]>(`${this.url}/user/${userId}`);
    }

    getMap(mapId: string): Observable<Map> {
        // console.log('getMap');
        // return of(this.getExampleMaps('1').find(map => map.id === mapId))
        return this.http.get<Map>(`${this.url}/${mapId}`)
            .pipe(
                tap(map => this.map$.next(map)),
                flatMap(map => this.getMapLayers(map.id)
                    .pipe(
                        tap(layers => this.layers$.next(layers)),
                        flatMap(() => of(map))
                    )
                )
            );
    }

    getMapLayers(mapId: string): Observable<Layer[]> {
        // console.log('getMapLayers');
        // return of(this.getExampleLayers(mapId));
        return this.http.get<Layer[]>(`${this.url}/${mapId}/layers`);
    }

    saveMap(map: SaveMapRequest): Observable<any> {
        // console.log('saveMap');
        // return of(null);
        return this.http.put<any>(this.url, map);
    }

    deleteMap(mapId: string): Observable<any> {
        // console.log('deleteMap');
        // return of(null)
        return this.http.delete<any>(`${this.url}/${mapId}`)
            .pipe(tap(() => this.closeMap()));
    }

    closeMap(): void {
        this.map$.next(null);
        this.layers$.next([]);
    }

    private getExampleMaps(userId: string): Map[] {
        return [
            {id: '1', name: 'New map 1', settings: '', description: 'simple description', userId},
            {id: '2', name: 'New map 2', settings: '', description: 'simple description', userId},
            {
                id: '3',
                name: 'Very long name Very long name Very long name Very long name',
                settings: '',
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
