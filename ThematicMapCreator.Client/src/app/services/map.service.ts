import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { flatMap, tap } from 'rxjs/operators';

import { environment } from '../../environments/environment';

import { Map } from '../models/map';
import { Layer } from '../models/layer';
import { SaveMapRequest } from '../contracts/save-map-request';
import { LayerType } from '../models/layer-type.enum';
import { LayerStyle } from '../models/layer-style.enum';

@Injectable({
    providedIn: 'root'
})
export class MapService {

    private apiUrl = environment.appUrl;
    private url = `${this.apiUrl}/api/maps`;

    public map$: BehaviorSubject<Map> = new BehaviorSubject<Map>(null);
    public layers$: BehaviorSubject<Layer[]> = new BehaviorSubject<Layer[]>([]);

    public zoomAll$: BehaviorSubject<any> = new BehaviorSubject<any>(null);
    public loading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    constructor(private http: HttpClient) {
    }

    getMaps(userId: string): Observable<Map[]> {
        // return of(this.getExampleMaps(userId));
        return this.http.get<Map[]>(`${this.url}/user/${userId}`);
    }

    getMap(mapId: string): Observable<Map> {
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
        // return of(this.getExampleLayers(mapId));
        return this.http.get<Layer[]>(`${this.url}/${mapId}/layers`);
    }

    saveMap(map: SaveMapRequest): Observable<Map> {
        // return of(null);
        return this.http.put<string>(this.url, map)
            .pipe(flatMap(mapId => this.getMap(mapId)));
    }

    deleteMap(mapId: string): Observable<any> {
        // return of(null)
        if (!mapId) {
            return of(null).pipe(tap(() => this.closeMap()));
        }

        return this.http.delete<any>(`${this.url}/${mapId}`)
            .pipe(tap(() => mapId === this.map$.getValue().id ? this.closeMap() : {}));
    }

    closeMap(): void {
        this.map$.next(null);
        this.layers$.next([]);
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
            {id: '1', index: 1, name: 'layer 1', visible: true, type: LayerType.None, style: LayerStyle.None, data: null, mapId},
            {id: '2', index: 2, name: 'layer 2', visible: true, type: LayerType.None, style: LayerStyle.None, data: null, mapId},
            {
                id: '3', index: 3, visible: false, type: LayerType.None, style: LayerStyle.None, data: null, mapId,
                name: 'layer 3 with very long name, layer 3 with very long name'
            },
        ];
    }
}
