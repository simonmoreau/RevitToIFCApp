import { Injectable } from '@angular/core';

import { Observable, BehaviorSubject } from 'rxjs';
import { BroadcastService, MsalService} from '@azure/msal-angular';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private forgeTokenSubject: BehaviorSubject<IForgeToken>;
  public currentForgeToken: Observable<IForgeToken>;

  constructor(private authService: MsalService) {
    const token: IForgeToken = { value : 'eyJhbGciOiJIUzI1NiIsImtpZCI6Imp3dF9zeW1tZXRyaWNfa2V5In0.eyJzY29wZSI6WyJjb2RlOmFsbCIsImRhdGE6d3JpdGUiLCJkYXRhOnJlYWQiLCJidWNrZXQ6Y3JlYXRlIiwiYnVja2V0OmRlbGV0ZSIsImJ1Y2tldDpyZWFkIl0sImNsaWVudF9pZCI6Imp2UU5UcVNZZkt6bXRVbGE2NFZxR3RQVE1HVUgyeHhDIiwiYXVkIjoiaHR0cHM6Ly9hdXRvZGVzay5jb20vYXVkL2p3dGV4cDYwIiwianRpIjoiSmR4c2xHaU5vNHlVd3dvRkI5a1I4Tm9WY0JRTkhCYXJTbG5FT2ZMTmJTUzhwb0U0eThOdHp6V0ZTUjRxOU8wcyIsImV4cCI6MTYwMDQ2MDM2MX0.YM7GS_50LMElcOqmonbFXxQhYJn58XcPEo77Btvlrqg'};
    this.forgeTokenSubject = new BehaviorSubject<IForgeToken>(token);
    this.currentForgeToken = this.forgeTokenSubject.asObservable();
   }

  public get currentTokenValue(): IForgeToken {
    return this.forgeTokenSubject.value;
  }

  public refreshToken(): Observable<IForgeToken> {

    // We must refresh the token before using the user
    return this.currentForgeToken;
  }

  Logout() {
    this.authService.logout();
  }
}

export interface IForgeToken {
  value: string;
}


