import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';

import { User } from '../models/user';
import { AuthorizationContract } from '../contracts/authorization-contract';
import { RegistrationContract } from '../contracts/registration-contract';

@Injectable({
    providedIn: 'root'
})
export class AuthorizationService {

    private url = 'api/authorization';

    public currentUser$: BehaviorSubject<User> = new BehaviorSubject<User>(null);

    constructor(private http: HttpClient) {
    }

    getCurrentUserId(): Observable<string> {
        return this.currentUser$.pipe(map(user => user?.id));
    }

    login(auth: AuthorizationContract): Observable<boolean> {
        // TODO return this.http.post<User>(`${this.url}/login`, auth)
        return of(this.getExampleUser())
            .pipe(
                tap(user => this.currentUser$.next(user)),
                map(user => !!user));
    }

    logout(): Observable<boolean> {
        // TODO return this.http.get<boolean>(`${this.url}/logout`)
        return of(true)
            .pipe(tap(result => result ? this.currentUser$.next(null) : {}));
    }

    signin(reg: RegistrationContract): Observable<boolean> {
        // TODO return this.http.post<boolean>(`${this.url}/signin`, reg);
        return of(true);
    }

    private getExampleUser(): User {
        return {
            id: '1',
            name: 'User',
            email: 'user@mail.com',
        };
    }
}
