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

    private url = 'https://localhost:5001/api/authorization';

    public currentUser$: BehaviorSubject<User> = new BehaviorSubject<User>(null);
    public logout$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(null);

    constructor(private http: HttpClient) {
    }

    getCurrentUserId(): Observable<string> {
        return this.currentUser$.pipe(map(user => user?.id));
    }

    login(auth: AuthorizationContract): Observable<boolean> {
        return this.http.post<User>(`${this.url}/login`, auth)
        // return of(this.getExampleUser())
            .pipe(
                tap(user => this.currentUser$.next(user)),
                map(user => !!user)
            );
    }

    logout(): Observable<boolean> {
        return this.http.get<boolean>(`${this.url}/logout`)
        // return of(true)
            .pipe(tap(result => {
                if (result) {
                    this.currentUser$.next(null);
                    this.logout$.next(true);
                }
            }));
    }

    signin(reg: RegistrationContract): Observable<boolean> {
        return this.http.post<User>(`${this.url}/signin`, reg)
            .pipe(map(user => !!user));
        // return of(true);
    }

    private getExampleUser(): User {
        return {
            id: '1',
            name: 'User',
            email: 'user@mail.com',
        };
    }
}
