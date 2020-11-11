import { Injectable } from '@angular/core';

import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { BroadcastService, MsalService } from '@azure/msal-angular';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpResponse,
} from '@angular/common/http';
import { catchError, map, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private forgeTokenSubject: BehaviorSubject<IForgeToken>;
  public currentForgeToken: Observable<IForgeToken>;

  public baseAPIURL = '/api/';

  constructor(private authService: MsalService, private http: HttpClient) {
    let token: IForgeToken = JSON.parse(localStorage.getItem('forgeToken'));

    if (!token) {

      const newToken: IForgeToken = {
        access_token: '',
        expires_in: 3600,
        token_type: 'bearer',
      };

      token = newToken;
    }

    this.forgeTokenSubject = new BehaviorSubject<IForgeToken>(token);
    this.currentForgeToken = this.forgeTokenSubject.asObservable();

  }

  public get currentTokenValue(): IForgeToken {
    return this.forgeTokenSubject.value;
  }

  public refreshToken(): Observable<IForgeToken> {
    // We must refresh the token before using the user
    return this.getForgeUploadToken().pipe(
      tap(t => localStorage.setItem('forgeToken', JSON.stringify(t)))
    );
  }

  LoginToForge(): Observable<IForgeToken> {
    return this.getForgeUploadToken().pipe(
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
  }

  private getForgeUploadToken(): Observable<IForgeToken> {
    return this.get<IForgeToken>(this.baseAPIURL + 'uploadToken');
  }

  private get<T>(url: string): Observable<T> {
    return this.http
      .get<T>(url, {
        headers: new HttpHeaders().set('Content-Type', 'application/json'),
      })
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      console.error(
        `Backend returned code ${error.status}, ` + `body was: ${error.error}`
      );
    }
    // Return an observable with a user-facing error message.
    return throwError('Something bad happened; please try again later.');
  }
}

export interface IForgeToken {
  access_token: string;
  token_type: string;
  expires_in: number;
}
