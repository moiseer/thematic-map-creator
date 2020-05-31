import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { GeoJSON } from 'leaflet';
import { map, take } from 'rxjs/operators';

import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class FileService {

    private apiUrl = environment.appUrl;
    private url = `${this.apiUrl}/api/files`;

    constructor(private http: HttpClient) {
    }

    public readFileAsText(file: File): Observable<string> {
        const result = new Subject<string>();
        const reader = new FileReader();
        reader.onload = () => {
            result.next(reader.result as string);
        };

        reader.readAsText(file);

        return result.pipe(take(1));
    }

    public readFileAsGeoJson(file: File): Observable<GeoJSON.GeoJsonObject> {
        const extension = file.name.split('.').pop().toLowerCase();
        switch (extension) {
            case 'json':
            case 'geojson':
                return this.readFileAsText(file).pipe(
                    map(text => this.tryGetGeoJson(text))
                );
            case 'csv':
                return this.getGeoJsonFromCsv(file);
        }
    }

    private getGeoJsonFromCsv(csv: File): Observable<GeoJSON.GeoJsonObject> {
        const formData = new FormData();
        formData.append('file', csv);
        return this.http.post<GeoJSON.GeoJsonObject>(`${this.url}/csv`, formData);
    }

    private tryGetGeoJson(text: string): GeoJSON.GeoJsonObject {
        try {
            const object = JSON.parse(text);
            const geojson = object as GeoJSON.GeoJsonObject;
            return this.processFeatureProperties(geojson);
        } catch (error) {
            console.log(error);
            return null;
        }
    }

    private processFeatureProperties(geojson: GeoJSON.GeoJsonObject): GeoJSON.FeatureCollection | GeoJSON.Feature | GeoJSON.GeoJsonObject {
        switch (geojson.type) {
            case 'FeatureCollection': {
                const featureCollection = geojson as GeoJSON.FeatureCollection;
                const newFeatures: GeoJSON.Feature[] = [];

                for (const oldFeature of featureCollection.features) {
                    newFeatures.push(this.processFeatureProperties(oldFeature) as GeoJSON.Feature);
                }

                featureCollection.features = newFeatures;
                return featureCollection;
            }
            case 'Feature': {
                const feature = geojson as GeoJSON.Feature;
                const newProperties: { [key: string]: any } = {};

                for (const propertyName of Object.keys(feature.properties)) {
                    const property = feature.properties[propertyName];

                    if (!property) {
                        continue;
                    }

                    if (property && typeof property === 'object' && !Array.isArray(property)) {
                        for (const innerPropertyName of Object.keys(property)) {
                            const innerProperty = property[innerPropertyName];

                            if (!innerProperty) {
                                continue;
                            }

                            newProperties[innerPropertyName] = innerProperty;
                        }
                    } else {
                        newProperties[propertyName] = property;
                    }
                }

                feature.properties = newProperties;
                return feature;
            }
            default:
                return geojson;
        }
    }
}
