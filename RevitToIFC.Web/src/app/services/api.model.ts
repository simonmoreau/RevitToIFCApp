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
  Created = 0,
  NotEnoughCredit,
  Error,
}

export interface IWorkItemStatus {
  status: string;
  progress: string;
  reportUrl: string;
  id: string;
}

export interface IWorkItemStatusEntity {
  progress: string;
  reportUrl: string;
  fileUrl: string;
  stats: string;
  status: string;
  userId: string;
  timeQueued: Date;
  size: number;
  version: string;
  fileName: string;
  timeDownloadStarted: Date;
  timeInstructionsStarted: Date;
  timeInstructionsEnded: Date;
  timeUploadEnded: Date;
  timeFinished: Date;
  bytesDownloaded: number;
  bytesUploaded: number;
}

export interface ConversionObject {
  uploadObjectResult: UploadObjectResult;
  workItemResponse: IWorkItemResponse | null;
  worfItemStatus: IWorkItemStatus | null;
}

export interface IMessage {
  name: string;
}

export interface ICheckoutSessionId {
  id: string;
}

export interface IConversionCreditsUpdate {
  userId: string;
  creditsNumber: number;
}
