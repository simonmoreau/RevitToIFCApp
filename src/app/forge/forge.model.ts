export interface IGetActivities {
  paginationToken: string;
  data: string[];
}

export interface IMessage {
  name: string;
}

export interface IUploadObject {
  bucketKey?: string;
  objectId?: string;
  objectKey?: string;
  sha1?: string;
  size?: number;
  contentType?: string;
  location?: string;
  [k: string]: unknown;
  nbChunks?: number;
}

