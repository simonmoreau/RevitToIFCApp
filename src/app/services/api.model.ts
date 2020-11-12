export interface IForgeToken {
  access_token: string;
  token_type: string;
  expires_in: number;
}

export interface RvtFile {
  url: string;
}

export interface Param {
  url: string;
}

export interface Result {
  verb: string;
  url: string;
}

export interface Arguments {
  rvtFile: RvtFile;
  param: Param;
  result: Result;
}

export interface WorkItemDescription {
  activityId: string;
  arguments: Arguments;
}
