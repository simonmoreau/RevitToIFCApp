import { Component, OnInit } from '@angular/core';
import { FileUploader, UploadObjectResult } from '../file-upload/file-uploader.class';
import { ForgeService } from '../forge/forge.service';
import { UserService } from '../services/user.service';
import { ApiService } from '../services/api.service';

import { IUploadObject } from '../forge/forge.model';

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

    this.uploader.response.subscribe((uploadObjectResult: UploadObjectResult) => {
      this.apiService.CreateWorkItem(uploadObjectResult.uploadObject.objectKey).subscribe(r => console.log(r));
    });
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
