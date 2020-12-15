import { Component, Input, OnInit } from '@angular/core';
import { MsalService} from '@azure/msal-angular';
import { ApiService } from '../services/api.service';
import { Account} from 'msal/lib-commonjs/Account';

@Component({
  selector: 'app-credits-counter',
  templateUrl: './credits-counter.component.html',
  styleUrls: ['./credits-counter.component.scss']
})
export class CreditsCounterComponent implements OnInit {

  loggedIn: boolean;
  userName: string;
  account: Account;
  credits: number;
  zeroCredits: boolean;
  oneCredits: boolean;
  moreCredits: boolean;
  isLoading: boolean;

  constructor(private authService: MsalService, private apiService: ApiService) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.checkAccount();
    this.apiService.GetConversionCredits(this.account.accountIdentifier).subscribe(c => {
      this.credits = c.creditsNumber;
      this.updateVisibility(this.credits);
      this.isLoading = false;
    });
  }

  checkAccount() {
    this.account = this.authService.getAccount();
    this.loggedIn = !!this.account;
    if (this.account) {
      this.userName = this.account.name;
    }
  }

  updateVisibility(creditsNumber: number): void {
    if (!creditsNumber)
    {
      this.credits = 0;
      this.zeroCredits = true;
      this.oneCredits = false;
      this.moreCredits = false;
    }
    else if (creditsNumber == 0)
    {
      this.zeroCredits = true;
      this.oneCredits = false;
      this.moreCredits = false;
    }
    else if (creditsNumber == 1)
    {
      this.zeroCredits = false;
      this.oneCredits = true;
      this.moreCredits = false;
    }
    else
    {
      this.zeroCredits = false;
      this.oneCredits = false;
      this.moreCredits = true;
    }
  }

}
