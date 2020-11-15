import { Component, OnInit } from '@angular/core';
import { FileUploader, UploadObjectResult } from '../file-upload/file-uploader.class';
import { ForgeService } from '../forge/forge.service';
import { UserService } from '../services/user.service';
import { ApiService } from '../services/api.service';


import { IUploadObject } from '../forge/forge.model';
import { IForgeToken, IWorkItemResponse, IWorkItemStatus, ConversionObject } from '../services/api.model';
import { concatMap, flatMap, takeWhile, switchMap, tap, map, first } from 'rxjs/operators';
import { RtlScrollAxisType } from '@angular/cdk/platform';
import { merge, Observable, of, timer } from 'rxjs';
import { FileItem } from '../file-upload/file-item.class';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {

  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  hasAnotherDropZoneOver: boolean;

  constructor(private userService: UserService, private forgeService: ForgeService, private apiService: ApiService){
    const bucketKey = 'ifc-storage';
    const objectName = 'input-revit-model';
    const URL = forgeService.forgeURL + `/oss/v2/buckets/${bucketKey}/objects/${objectName}`;

    this.uploader = new FileUploader({
      url: URL,
      method: 'PUT',
      // headers: [{ name: 'Authorization', value: `Bearer ${userService.currentTokenValue?.access_token}` }],
      disableMultipart: true, // 'DisableMultipart' must be 'true' for formatDataFunction to be called.
      formatDataFunctionIsAsync: true,
      formatDataFunction: async (item) => {
        return new Promise( (resolve, reject) => {
          resolve({
            name: item._file.name,
            length: item._file.size,
            contentType: item._file.type,
            date: new Date()
          });
        });
      }
    }, forgeService);

    this.hasBaseDropZoneOver = false;
    this.hasAnotherDropZoneOver = false;

    // this.uploader.response.subscribe( response: IUploadObject => {this.response = response ; console.log(response); } );

    const createWorkItemObs = (conversionObject: ConversionObject): Observable<ConversionObject> => {
      const currentFileItem: FileItem = conversionObject.uploadObjectResult.file;
      currentFileItem.isConverting = true;
      currentFileItem.status = 'Converting ...' + currentFileItem.file.name;
      return this.apiService.CreateWorkItem(conversionObject.uploadObjectResult.uploadObject.objectKey).pipe(
        map((workItemResponse: IWorkItemResponse ) => {
          conversionObject.uploadObjectResult.file.downloadUrl = workItemResponse.outputUrl;
          conversionObject.workItemResponse = workItemResponse;
          return conversionObject;
        })
      );
    };

    const getworkItemStatus = (conversionObject: ConversionObject): Observable<ConversionObject> => {
      return of (conversionObject).pipe(
        concatMap((cO: ConversionObject) => checkStatus(cO)),
        concatMap((cO: ConversionObject) => processConvertedObject(cO)),
      );
    };

    const checkStatus = (conversionObject: ConversionObject): Observable<ConversionObject> => {
      return timer(0, 2000).pipe(
        switchMap(() => this.apiService.GetWorkItemStatus(conversionObject.workItemResponse.workItemId)),
        first(workItemStatus => workItemStatus.status === 'success'),
        map(r => conversionObject)
        );
    };

    const processConvertedObject = (conversionObject: ConversionObject): Observable<ConversionObject> => {
      return  of(conversionObject).pipe(
        map((cO: ConversionObject) => {
          cO.uploadObjectResult.file.isConverting = false;
          cO.uploadObjectResult.file.status = 'Converted';
          cO.uploadObjectResult.file.isConverted = true;
          return cO;
        })
      );
    };

    const test = this.uploader.response.pipe(
      map((uor: UploadObjectResult ) =>  {
        const conversionObject: ConversionObject = {
          uploadObjectResult: uor,
          workItemResponse: null
      };
        return conversionObject;
      }),
      flatMap((conversionObject: ConversionObject) => createWorkItemObs(conversionObject)),
      flatMap( (conversionObject: ConversionObject) => getworkItemStatus(conversionObject))
    );

    test.subscribe(r => console.log(r));

  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  public fileOverAnother(e: any): void {
    this.hasAnotherDropZoneOver = e;
  }

  ngOnInit(): void {
    this.userService.refreshToken().subscribe(t => console.log('Get a Forge Token'));
  }

}

