import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpResponse,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { IForgeToken, IWorkItemResponse, IWorkItemStatus, IMessage, ICheckoutSessionId, IConversionCreditsUpdate } from './api.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  public baseAPIURL = environment.apiUri + '/api/';

  constructor(private http: HttpClient) {}

  public getForgeUploadToken(): Observable<IForgeToken> {
    return this.get<IForgeToken>(this.baseAPIURL + 'uploadToken');
  }

  public getLocalAPI(): Observable<IMessage> {
    return this.get<IMessage>(this.baseAPIURL + 'message');
  }

  public createCheckoutSession(userId: string, productId: string ): Observable<ICheckoutSessionId> {

    const body = { userId, productId };

    return this.post<ICheckoutSessionId>(this.baseAPIURL + 'checkoutSession', body);
  }

  public updateConversionCredits(userId: string, sessionId: string ): Observable<IConversionCreditsUpdate> {

    const body = { userId, sessionId };

    return this.post<IConversionCreditsUpdate>(this.baseAPIURL + 'credits', body);
  }

  public GetConversionCredits(userId: string ): Observable<IConversionCreditsUpdate> {

    return this.get<IConversionCreditsUpdate>(this.baseAPIURL + 'credits?id=' + userId);
  }

  public CreateWorkItem(
    objectName: string,
    outputName: string,
    forgeActivityId: string,
    currentUserId: string,
    currentFileSize: number
  ): Observable<IWorkItemResponse> {
    const body = { inputObjectName: objectName, outputObjectName: outputName, activityId: forgeActivityId , userId: currentUserId, fileSize: currentFileSize};
    return this.post<IWorkItemResponse>(
      this.baseAPIURL + 'workitem',
      body
    );
  }

  public GetWorkItemStatus(workItemId: string ): Observable<IWorkItemStatus> {
    return this.get<IWorkItemStatus>(this.baseAPIURL + 'workitem/' + workItemId);
  }

  // private function for REST requests
  private get<T>(url: string): Observable<T> {
    return this.http
      .get<T>(url, {
        headers: new HttpHeaders().set('Content-Type', 'application/json'),
      })
      .pipe(catchError(this.handleError));
  }

  private post<T>(url: string, body: object): Observable<T> {
    return this.http
      .post<T>(url, body, {
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
