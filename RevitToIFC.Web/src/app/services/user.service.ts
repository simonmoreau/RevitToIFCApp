import { Injectable } from '@angular/core';

import { Observable, BehaviorSubject, throwError, from, of } from 'rxjs';
import { MsalService } from '@azure/msal-angular';
import { map, tap } from 'rxjs/operators';

import { IForgeToken } from './api.model';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private forgeTokenSubject: BehaviorSubject<IForgeToken | null>;
  public currentForgeToken: Observable<IForgeToken | null>;

  constructor(
    private authService: MsalService,
    private apiService: ApiService
  ) {
    this.forgeTokenSubject = new BehaviorSubject<IForgeToken | null>(
      JSON.parse(localStorage.getItem('forgeToken')!)
    );
    this.currentForgeToken = this.forgeTokenSubject.asObservable();
  }

  public get currentTokenValue(): IForgeToken | null {
    return this.forgeTokenSubject.value;
  }

  public getToken(): Observable<IForgeToken> {

    const savedToken: IForgeToken = JSON.parse(localStorage.getItem('forgeToken')!);
    if (savedToken)
    {
      return of(savedToken);
    }
    else
    {
      return this.refreshToken();
    }
  }

  public refreshToken(): Observable<IForgeToken> {
    // We must refresh the token before using the user
    return this.apiService.getForgeUploadToken().pipe(
      tap((t) => {
        localStorage.setItem('forgeToken', JSON.stringify(t));
        this.forgeTokenSubject.next(t);
      })
    );
  }

  LoginToForge(): Observable<IForgeToken> {
    return this.apiService.getForgeUploadToken().pipe(
      map((forgeToken) => {
        // login successful if there's a jwt token in the response
        if (forgeToken) {
          // store user details and jwt token in local storage to keep user logged in between page refreshes
          localStorage.setItem('forgeToken', JSON.stringify(forgeToken));
          this.forgeTokenSubject.next(forgeToken);
        }
        return forgeToken;
      })
    );
  }

  Logout() {
    this.authService.logout();
    localStorage.removeItem('forgeToken');
    this.forgeTokenSubject.next(null);
  }
}
