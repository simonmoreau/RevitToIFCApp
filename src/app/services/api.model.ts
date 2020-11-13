export interface IForgeToken {
  access_token: string;
  token_type: string;
  expires_in: number;
}

export interface  IWorkItemStatusResponse
{
    WorkItemId: string;
    OutputUrl: string;
}
