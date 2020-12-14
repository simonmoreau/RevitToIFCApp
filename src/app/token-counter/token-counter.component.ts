import { Component, Input, OnInit } from '@angular/core';
import { MsalService} from '@azure/msal-angular';
import { ApiService } from '../services/api.service';
import { Account} from 'msal/lib-commonjs/Account';
@Component({
  selector: 'app-token-counter',
  templateUrl: './token-counter.component.html',
  styleUrls: ['./token-counter.component.scss']
})
export class TokenCounterComponent implements OnInit {

  loggedIn: boolean;
  userName: string;
  account: Account;
  tokens: number;

  constructor(private authService: MsalService, private apiService: ApiService) { }

  ngOnInit(): void {
    this.checkAccount();
    this.apiService.GetConversionToken(this.account.accountIdentifier).subscribe(t => {
      this.tokens = t.tokenNumber;
    });
  }

  checkAccount() {
    this.account = this.authService.getAccount();
    this.loggedIn = !!this.account;
    if (this.account) {
      this.userName = this.account.name;
    }
  }

}
