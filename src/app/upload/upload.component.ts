import { Component, OnInit } from '@angular/core';
import { FileUploader } from '../file-upload/file-uploader.class';
import { ForgeService } from '../forge/forge.service';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {

  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  hasAnotherDropZoneOver: boolean;
  response: string;

  constructor(private userService: UserService, private forgeService: ForgeService){
    const bucketKey = 'ifc-storage';
    const objectName = 'input-revit-model';
    const URL = forgeService.baseURL + `/oss/v2/buckets/${bucketKey}/objects/${objectName}`;

    this.uploader = new FileUploader({
      url: URL,
      method: 'PUT',
      headers: [{ name: 'Authorization', value: `Bearer ${userService.currentTokenValue.value}` }],
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

    this.response = '';

    this.uploader.response.subscribe( res => {this.response = res ; console.log(res); } );
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  public fileOverAnother(e: any): void {
    this.hasAnotherDropZoneOver = e;
  }

  ngOnInit(): void {
  }

}
