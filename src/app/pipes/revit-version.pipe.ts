import { Pipe, PipeTransform } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Pipe({
  name: 'revitVersion'
})
export class RevitVersionPipe implements PipeTransform {

  versionBehaviorSubject = new BehaviorSubject<string>('Looking for the Revit version');

  transform(file: File): BehaviorSubject<string> {

    if (!file) {
      return;
    }

    const opt: IParseFileOption = {
      chunk_size: 64 * 1024,
      binary: true,
      error_callback: (result: string | ArrayBuffer) => { console.log('error'); console.log(result); },
      chunk_read_callback: (result: string | ArrayBuffer) => this.FindRevitVersionInText(result),
    };

    this.parseFile(file, opt);

    return this.versionBehaviorSubject;
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

    const opts = typeof options === 'undefined' ? {} : options;
    const fileSize = file.size;
    const chunkSize = typeof opts['chunk_size'] === 'undefined' ? 64 * 1024 : parseInt(opts['chunk_size']); // bytes
    const binary = typeof opts['binary'] === 'undefined' ? false : opts['binary'] === true;
    let offset = 0;
    let readBlock = null;
    const chunkReadCallback = typeof opts['chunk_read_callback'] === 'function' ? opts['chunk_read_callback'] : function () { };
    const chunkErrorCallback = typeof opts['error_callback'] === 'function' ? opts['error_callback'] : function () { };

    const onLoadHandler = (evt: ProgressEvent<FileReader>) => {
      let stopReading = false;
      if (evt.target.error == null) {
        offset += evt.target.result.toString().length;
        stopReading = chunkReadCallback(evt.target.result);
      } else {
        chunkErrorCallback(evt.target.error);
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

  private FindRevitVersionInText(contents: string | ArrayBuffer): boolean {

    console.log('readRevitAsText');
    const line: string = contents.toString().replace(/[^ -~]+/g, '');
    if (line.includes('Format:')) {
      const regex = /Format: (\d+)/g;
      const found = line.match(regex);
      const version = found[0].replace('Format: ', '');
      console.log(version);
      this.versionBehaviorSubject.next(version);
      return true;
    }
    else if (line.includes('Revit Build: ')) {
      const regex = /Revit Build: Autodesk Revit (\d+)/g;
      const found = line.match(regex);
      const version = found[0].replace('Revit Build: Autodesk Revit ', '');
      console.log(version);
      this.versionBehaviorSubject.next(version);
      return true;
    }
    else {
      return false;
    }
  }
}

interface IParseFileOption {
  chunk_size: number;
  binary: boolean;
  chunk_read_callback: (result: string | ArrayBuffer) => boolean;
  error_callback: (result: string | ArrayBuffer) => void;
}

