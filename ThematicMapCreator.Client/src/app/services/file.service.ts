import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { geoJSON, GeoJSON } from 'leaflet';
import { map } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class FileService {

    public readFileAsText(file: File): Observable<string> {
        const result = new Subject<string>();
        const reader = new FileReader();
        reader.onload = () => {
            result.next(reader.result as string);
        };

        reader.readAsText(file);

        return result.asObservable();
    }

    public readFileAsGeoJson(file: File): Observable<GeoJSON.GeoJsonObject>  {
        return this.readFileAsText(file).pipe(
            map(text => this.tryGetGeoJson(text))
        );
    }

    private tryGetGeoJson(text: string): GeoJSON.GeoJsonObject {
        try {
            const object = JSON.parse(text);
            return object
                ? geoJSON(object) ? object : null
                : null;
        } catch {
            return null;
        }
    }
}
