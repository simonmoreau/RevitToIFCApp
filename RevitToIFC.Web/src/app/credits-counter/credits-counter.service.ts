import { EventEmitter, Injectable } from '@angular/core';
import { MsalService } from '@azure/msal-angular';
import { AccountInfo } from '@azure/msal-browser';

import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IConversionCreditsUpdate } from '../services/api.model';
import { ApiService } from '../services/api.service';

@Injectable({
  providedIn: 'root',
})
export class CreditsCounterService {

  private accountInfo!: AccountInfo;
  public creditCount: number = 0;
  
  public displayedCreditsEvent: EventEmitter<number>;
  public displayedCredits: number = 0;
  public userName: string = '';

  constructor(
    private authService: MsalService,
    private apiService: ApiService
  ) {
    this.displayedCreditsEvent = new EventEmitter<number>();
  }

  public UpdateConversionCredits(checkoutSessionId: string): Observable<IConversionCreditsUpdate> {

    this.checkAccount();
    if (this.accountInfo)
    {
      return this.apiService
      .updateConversionCredits(this.accountInfo.homeAccountId,checkoutSessionId).pipe(
        map((conversionCreditsUpdate: IConversionCreditsUpdate) => {
          this.creditCount = conversionCreditsUpdate.creditsNumber;
          this.displayedCredits = this.creditCount;
          this.displayedCreditsEvent.next(this.displayedCredits);
          return conversionCreditsUpdate;
        })
      );
    }
    else
    {
      throw new Error("Could not retrieve the account id, please contact simon@bim42.com");
    }

  }


  public UpdateDisplayedCredits(creditToAdd: number): number
  {
    this.displayedCredits = this.displayedCredits + creditToAdd;
    this.displayedCreditsEvent.next(this.displayedCredits);
    return this.displayedCredits;
  }

  public GetConversionCredits(): Observable<IConversionCreditsUpdate> {
    this.checkAccount();
    return this.apiService
      .GetConversionCredits(this.accountInfo.homeAccountId)
      .pipe(
        map((conversionCreditsUpdate: IConversionCreditsUpdate) => {

          this.creditCount = conversionCreditsUpdate.creditsNumber;
          this.displayedCredits = this.creditCount;
          this.displayedCreditsEvent.next(this.displayedCredits);

          return conversionCreditsUpdate;
        })
      );
  }

  checkAccount() {
    this.accountInfo = this.authService.instance.getActiveAccount()!;
    if (this.accountInfo) {
      this.userName = this.accountInfo.name!;
    }
  }
}
