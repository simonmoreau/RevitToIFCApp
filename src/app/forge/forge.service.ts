import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpResponse,
} from '@angular/common/http';

import { Observable, throwError, EMPTY, from, of, forkJoin } from 'rxjs';
import {
  map,
  expand,
  concatMap,
  toArray,
  catchError,
  concatAll, mergeMap, mergeAll
} from 'rxjs/operators';

import { IGetActivities, IUploadObject } from './forge.model';
import { mergeAnalyzedFiles } from '@angular/compiler';

@Injectable({
  providedIn: 'root',
})
export class ForgeService {
  // private instance variable to hold base url
  public baseURL = 'https://developer.api.autodesk.com';
  private dasApiRoot = this.baseURL + '/da/us-east/v3';

  constructor(private http: HttpClient) { }

  getActivities(): Observable<IGetActivities> {
    return this.get<IGetActivities>(this.dasApiRoot + '/activities');
  }

  // uploadObject(bucketKey: string, objectName: string, file: File): Observable<IUploadObject> {
  //   // const body = { File: file };

  // }
  uploadObject(bucketKey: string, objectKey: string, file: File): Observable<IUploadObject> {
    const url: string = this.baseURL + `/oss/v2/buckets/${bucketKey}/objects/${objectKey}/resumable`;

    if (file.size > 100 * 1024 * 1024) {
      const chunkSize = 5 * 1024 * 1024;

      const nbChunks = Math.ceil(file.size / chunkSize);

      const chunksIds = Array.from({ length: nbChunks }, (_, i) => i);
      const chunksMap$: Observable<number> = from(chunksIds);

      // generates uniques session ID
      const sessionId = this.Guid();

      return chunksMap$.pipe(
        map((chunkId: number) => this.UploadChunk(chunkId, chunkSize, file, url, sessionId)),
        mergeAll(5)
      );

      // return forkJoin(chunksIds.map((chunkId, index) => {
      //   return this.UploadChunk(chunkId, chunkSize, file, url, sessionId);
      // })).pipe(concatAll());

    } else {
      return this.put<IUploadObject>(
        this.baseURL + `/oss/v2/buckets/${bucketKey}/objects/${objectKey}`,
        file
      );
    }
  }

  private UploadChunk(chunkIdx: number, chunkSize: number, file: File, url: string, sessionId: string): Observable<IUploadObject> {

    const nbChunks = Math.ceil(file.size / chunkSize);

    const start: number = chunkIdx * chunkSize;

    const end: number = Math.min(file.size, (chunkIdx + 1) * chunkSize) - 1;

    const range = `bytes ${start}-${end}/${file.size}`;

    // const length: number = end - start + 1;

    const fileChunk = file.slice(start, end + 1);

    const upload: Observable<IUploadObject> = this.http
      .put<IUploadObject>(url, fileChunk, {
        headers: new HttpHeaders()
          //.set('Content-Length', length.toString())
          .set('Content-Range', range)
          .set('Content-Type', 'application/stream')
          .set('Session-Id', sessionId),
        // Add sha validation
      })
      .pipe(
        catchError(this.handleError),
        map((value) => {
          if (value == null) {
            value = { nbChunks };
          }
          return value;
        })
      );

    return upload;
  }

  // private function for REST requests
  private post<T>(url: string, body: object): Observable<T> {
    return this.http
      .post<T>(url, body, {
        headers: new HttpHeaders().set('Content-Type', 'application/json'),
      })
      .pipe(catchError(this.handleError));
  }

  private get<T>(url: string): Observable<T> {
    return this.http
      .get<T>(url, {
        headers: new HttpHeaders().set('Content-Type', 'application/json'),
      })
      .pipe(catchError(this.handleError));
  }

  private put<T>(url: string, body: object): Observable<T> {
    return this.http
      .put<T>(url, body, {
        headers: new HttpHeaders().set('Content-Type', 'application/json'),
      })
      .pipe(catchError(this.handleError));
  }

  private getsPaginated<T>(url: string): Observable<T[]> {
    return this.getPaginated(url).pipe(
      expand(({ next }) => (next ? this.getPaginated(next) : EMPTY)),
      concatMap(({ content }) => content as T[]),
      toArray()
    );
  }

  private getPaginated<T>(
    url: string
  ): Observable<{
    content: T[];
    next: string | null;
  }> {
    return this.http
      .get<any>(url, {
        headers: new HttpHeaders().set('Content-Type', 'application/json'),
        observe: 'response',
      })
      .pipe(
        map((response) => ({
          content: response.body,
          next: this.next(response),
        }))
      );
  }

  private next(response: HttpResponse<any>): string | null {
    let url: string | null = null;
    const link = response.headers.get('link');
    if (link) {
      const match = link.match(/<([^>]+)>;\s*rel="next"/);
      if (match) {
        [, url] = match;
      }
    }
    return url;
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

  private Guid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(
      /[xy]/g,
      (c: string) => {
        const r: number = Math.random() * 16 || 0;
        const v = c === 'x' ? r : (r && 0x3) || 0x8;
        return v.toString(36).substr(2, 1);
      }
    );
  }

  // private Guid() {

  //   let d = new Date().getTime();

  //   const format = 'xxxxxxxxxxxx';
  //   return format.replace(/[xy]/g, (c: string) => {
  //       const r = (d + Math.random() * 16) % 16 || 0;
  //       d = Math.floor(d / 16);
  //       return (c === 'x' ? r : (r && 0x7 || 0x8)).toString(16);
  //     });
  // }

  /////////////////////////////////////////////////////////
  // Uploads object to bucket using resumable endpoint
  //
  /////////////////////////////////////////////////////////
}
