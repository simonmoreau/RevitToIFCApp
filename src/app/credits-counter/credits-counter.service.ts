import { EventEmitter, Injectable } from '@angular/core';
import { MsalService } from '@azure/msal-angular';
import { Account } from 'msal/lib-commonjs/Account';

import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IConversionCreditsUpdate } from '../services/api.model';
import { ApiService } from '../services/api.service';

@Injectable({
  providedIn: 'root',
})
export class CreditsCounterService {
  private account: Account;
  public creditCount: number;
  
  public displayedCreditsEvent: EventEmitter<number>;
  public displayedCredits;
  public userName: string;

  constructor(
    private authService: MsalService,
    private apiService: ApiService
  ) {
    this.displayedCreditsEvent = new EventEmitter<number>();
  }

  public UpdateConversionCredits(checkoutSessionId: string): Observable<IConversionCreditsUpdate> {
    if (!this.account) {
      this.checkAccount();
    }

    return this.apiService
      .updateConversionCredits(this.account.accountIdentifier,checkoutSessionId).pipe(
        map((conversionCreditsUpdate: IConversionCreditsUpdate) => {
          this.creditCount = conversionCreditsUpdate.creditsNumber;
          this.displayedCredits = this.creditCount;
          this.displayedCreditsEvent.next(this.displayedCredits);
          return conversionCreditsUpdate;
        })
      );
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
      .GetConversionCredits(this.account.accountIdentifier)
      .pipe(
        map((conversionCreditsUpdate: IConversionCreditsUpdate) => {
          // TODO temp
          conversionCreditsUpdate.creditsNumber = 5;

          this.creditCount = conversionCreditsUpdate.creditsNumber;
          this.displayedCredits = this.creditCount;
          this.displayedCreditsEvent.next(this.displayedCredits);

          return conversionCreditsUpdate;
        })
      );
  }

  checkAccount() {
    this.account = this.authService.getAccount();
    if (this.account) {
      this.userName = this.account.name;
    }
  }
}
