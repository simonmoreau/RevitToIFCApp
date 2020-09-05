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
    const token: IForgeToken = { value : 'eyJhbGciOiJIUzI1NiIsImtpZCI6Imp3dF9zeW1tZXRyaWNfa2V5In0.eyJzY29wZSI6WyJjb2RlOmFsbCIsImRhdGE6d3JpdGUiLCJkYXRhOnJlYWQiLCJidWNrZXQ6Y3JlYXRlIiwiYnVja2V0OmRlbGV0ZSIsImJ1Y2tldDpyZWFkIl0sImNsaWVudF9pZCI6InB5eDhHU0x0czdiUzBHNUNFSjZvbVg5Q212VjNYbnprIiwiYXVkIjoiaHR0cHM6Ly9hdXRvZGVzay5jb20vYXVkL2p3dGV4cDYwIiwianRpIjoiUVNXYWluYWhkVnhTRldSTzJnQzBJR3NUT2Nqd0pySEs3WkE2RHZmMVMzZWp1SmcyWm5PSElnZjF6MHkzWlc0MyIsImV4cCI6MTU5OTMyMTQ4N30.F9O_YzrOcxYVg8qRp8E46C5f2h1cA5piIfHHqCl0oIE'};
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


