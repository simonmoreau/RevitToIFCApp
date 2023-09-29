import { BehaviorSubject, Observable, of } from 'rxjs';
import { FileLikeObject } from './file-like-object.class';
import {
  FileUploader,
  ParsedResponseHeaders,
  FileUploaderOptions,
} from './file-uploader.class';

export class FileItem {
  public file: FileLikeObject;
  public version: string = '';
  public _file: File;
  public alias: string = '';
  public url = '/';
  public method: string = '';
  public headers: any = [];
  public withCredentials = true;
  public formData: any = [];
  public isReady = false;
  public isProcessing = false;
  public isUploaded = false;
  public isSuccess = false;
  public isCancel = false;
  public isError = false;
  public progress = 0;
  public status = '';
  public isConverting = false;
  public isConverted = false;
  public downloadUrl: string = '';
  public index: number = 0;
  public _xhr!: XMLHttpRequest;
  public _form: any;

  protected uploader: FileUploader;
  protected some: File;
  protected options: FileUploaderOptions;

  public constructor(
    uploader: FileUploader,
    some: File,
    options: FileUploaderOptions
  ) {
    this.uploader = uploader;
    this.some = some;
    this.options = options;
    this.file = new FileLikeObject(some);
    this._file = some;
    if (uploader.options) {
      this.method = uploader.options.method || 'POST';
      this.alias = uploader.options.itemAlias || 'file';
    }
    this.url = uploader.options.url!;
    if (this.file?.size / 1024 / 1024 > 600) {
      this.isError = true;
      this.status =
        'The maximum Revit file size is 600 Mb, please upload a smaller file.';
    } else {
      this.status = 'Looking for the Revit version ...';
      this.isProcessing = true;
      this.getRevitVersion(some).subscribe((v) => {
        this.version = v;
        if (this.version !== '') {
          this.isProcessing = false;
          if (!this.isError) {
            this.status = 'Ready to be uploaded';
          }
        }
      });
    }
  }

  public upload(): void {
    try {
      this.uploader.uploadItem(this);
      this.status = 'Uploading ...';
    } catch (e) {
      this.uploader._onCompleteItem(this, '', 0, {});
      this.uploader._onErrorItem(this, '', 0, {});
    }
  }

  public cancel(): void {
    this.uploader.cancelItem(this);
  }

  public remove(): void {
    this.uploader.removeFromQueue(this);
  }

  public onBeforeUpload(): void {
    return void 0;
  }

  public onBuildForm(form: any): any {
    return { form };
  }

  public onProgress(progress: number): any {
    return { progress };
  }

  public onSuccess(
    response: string,
    status: number,
    headers: ParsedResponseHeaders
  ): any {
    return { response, status, headers };
  }

  public onError(
    response: string,
    status: number,
    headers: ParsedResponseHeaders
  ): any {
    return { response, status, headers };
  }

  public onCancel(
    response: string,
    status: number,
    headers: ParsedResponseHeaders
  ): any {
    return { response, status, headers };
  }

  public onComplete(
    response: string,
    status: number,
    headers: ParsedResponseHeaders
  ): any {
    return { response, status, headers };
  }

  public _onBeforeUpload(): void {
    this.isReady = true;
    this.isProcessing = true;
    this.isUploaded = false;
    this.isSuccess = false;
    this.isCancel = false;
    this.isError = false;
    this.progress = 0;
    this.onBeforeUpload();
  }

  public _onBuildForm(form: any): void {
    this.onBuildForm(form);
  }

  public _onProgress(progress: number): void {
    this.progress = progress;
    this.onProgress(progress);
  }

  public _onSuccess(
    response: string,
    status: number,
    headers: ParsedResponseHeaders
  ): void {
    this.isReady = false;
    this.isProcessing = false;
    this.isUploaded = true;
    this.isSuccess = true;
    this.isCancel = false;
    this.isError = false;
    this.progress = 100;
    this.index = 0;
    this.onSuccess(response, status, headers);
  }

  public _onError(
    response: string,
    status: number,
    headers: ParsedResponseHeaders
  ): void {
    this.isReady = false;
    this.isProcessing = false;
    this.isUploaded = true;
    this.isSuccess = false;
    this.isCancel = false;
    this.isError = true;
    this.progress = 0;
    this.index = 0;
    this.onError(response, status, headers);
  }

  public _onCancel(
    response: string,
    status: number,
    headers: ParsedResponseHeaders
  ): void {
    this.isReady = false;
    this.isProcessing = false;
    this.isUploaded = false;
    this.isSuccess = false;
    this.isCancel = true;
    this.isError = false;
    this.progress = 0;
    this.index = 0;
    this.onCancel(response, status, headers);
  }

  public _onComplete(
    response: string,
    status: number,
    headers: ParsedResponseHeaders
  ): void {
    this.onComplete(response, status, headers);

    if (this.uploader.options.removeAfterUpload) {
      this.remove();
    }
  }

  public _prepareToUploading(): void {
    this.index = this.index || ++this.uploader._nextIndex;
    this.isReady = true;
  }

  private getRevitVersion(file: File): Observable<string> {
    if (!file) {
      throw new Error("No files");
    }

    const versionSubject: BehaviorSubject<string> = new BehaviorSubject<string>(
      ''
    );
    const versionObservable: Observable<string> = versionSubject.asObservable();

    const opt: IParseFileOption = {
      chunk_size: 64 * 1024,
      binary: true,
      error_callback: (result: string | ArrayBuffer) => {
        console.log('error');
        console.log(result);
      },
      chunk_read_callback: (result: string | ArrayBuffer) => {
        const version = this.FindRevitVersionInText(result);
        if (version !== null) {
          versionSubject.next(version);
        }
      },
    };

    this.parseFile(file, opt);

    return versionObservable;
  }

  /*
 * Valid options are:
 * - chunk_read_callback: a function that accepts the read chunk
                          as its only argument. If binary option
                          is set to true, this function will receive
                          an instance of ArrayBuffer, otherwise a String
 * - error_callback:      an optional function that accepts an object of type
                          FileReader.error
 * - success:             an optional function invoked as soon as the whole file has been
                          read successfully
 * - binary:              If true chunks will be read through FileReader.readAsArrayBuffer
 *                        otherwise as FileReader.readAsText. Default is false.
 * - chunk_size:          The chunk size to be used, in bytes. Default is 64K.
 */
  private parseFile(file: File, options: IParseFileOption) {
    const opts: any = typeof options === 'undefined' ? {} : options;
    const fileSize = file.size;
    const chunkSize =
      typeof opts['chunk_size'] === 'undefined'
        ? 64 * 1024
        : parseInt(opts['chunk_size']); // bytes
    const binary =
      typeof opts['binary'] === 'undefined' ? false : opts['binary'] === true;
    let offset = 0;
    let readBlock: any = null;
    const chunkReadCallback =
      typeof opts['chunk_read_callback'] === 'function'
        ? opts['chunk_read_callback']
        : function () { };
    const chunkErrorCallback =
      typeof opts['error_callback'] === 'function'
        ? opts['error_callback']
        : function () { };

    const onLoadHandler = (evt: ProgressEvent<FileReader>) => {
      let stopReading = false;
      if (evt.target!.error == null) {
        offset += evt.target!.result!.toString().length;
        stopReading = chunkReadCallback(evt.target!.result);
      } else {
        chunkErrorCallback(evt.target!.error);
        return;
      }
      if (offset >= fileSize) {
        return;
      }
      if (!stopReading) {
        readBlock(offset, chunkSize, file);
      }
    };

    readBlock = (offset: number, length: number, file: File) => {
      const r = new FileReader();
      const blob: Blob = file.slice(offset, length + offset);
      r.onload = onLoadHandler;
      if (binary) {
        r.readAsBinaryString(blob);
      } else {
        r.readAsText(blob);
      }
    };

    readBlock(offset, chunkSize, file);
  }

  private FindRevitVersionInText(contents: string | ArrayBuffer): string {
    const line: string = contents.toString().replace(/[^ -~]+/g, '');
    if (line.includes('Format:')) {
      const regex = /Format: (\d+)/g;
      const found = line.match(regex);
      const version = found![0].replace('Format: ', '');
      console.log(version);
      return version;
    } else if (line.includes('Revit Build: ')) {
      const regex = /Revit Build: Autodesk Revit (\d+)/g;
      const found = line.match(regex);
      let version = null;
      if (found) {
        version = found[0].replace('Revit Build: Autodesk Revit ', '');
      } else {
        if (line.includes('Architecture')) {
          const regex = /Revit Build: Autodesk Revit Architecture (\d+)/g;
          const found = line.match(regex);
          if (found) {
            version = found[0].replace(
              'Revit Build: Autodesk Revit Architecture ',
              ''
            );
          }
        } else if (line.includes('MEP')) {
          const regex = /Revit Build: Autodesk Revit MEP (\d+)/g;
          const found = line.match(regex);
          if (found) {
            version = found[0].replace(
              'Revit Build: Autodesk Revit MEP ',
              ''
            );
          }
        } else if (line.includes('Structure')) {
          const regex = /Revit Build: Autodesk Revit Structure (\d+)/g;
          const found = line.match(regex);
          if (found) {
            version = found[0].replace(
              'Revit Build: Autodesk Revit Structure ',
              ''
            );
          }
        }
      }

      console.log(version);
      if (version === null) throw new Error('Version is empty');
      return version;
    } else {
      throw new Error('Version is empty');
    }
  }
}

interface IParseFileOption {
  chunk_size: number;
  binary: boolean;
  chunk_read_callback: (result: string | ArrayBuffer) => void;
  error_callback: (result: string | ArrayBuffer) => void;
}
