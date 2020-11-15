import { UploadObjectResult } from '../file-upload/file-uploader.class';

export interface IForgeToken {
  access_token: string;
  token_type: string;
  expires_in: number;
}

export interface IWorkItemResponse {
  workItemId: string;
  outputUrl: string;
}

export interface IWorkItemStatus {
  status: string;
  progress: string;
  reportUrl: string;
  id: string;
}


export interface ConversionObject {
  uploadObjectResult: UploadObjectResult;
  workItemResponse: IWorkItemResponse;
}
