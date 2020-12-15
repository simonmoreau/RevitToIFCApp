import { UploadObjectResult } from '../file-upload/file-uploader.class';

export interface IForgeToken {
  access_token: string;
  token_type: string;
  expires_in: number;
}

export interface IWorkItemResponse {
  workItemId: string;
  outputUrl: string;
  workItemCreationStatus: WorkItemCreationStatus;
}

export enum WorkItemCreationStatus {
  Created = 0, NotEnoughCredit, Error
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
  worfItemStatus: IWorkItemStatus;
}

export interface IMessage {
  name: string;
}

export interface ICheckoutSessionId {
  id: string;
}

export interface IConversionTokenUpdate {
  userId: string;
  creditsNumber: number;
}

