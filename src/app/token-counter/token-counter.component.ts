import { Component, Input, OnInit } from '@angular/core';
import { MsalService } from '@azure/msal-angular';

@Component({
  selector: 'app-token-counter',
  templateUrl: './token-counter.component.html',
  styleUrls: ['./token-counter.component.scss']
})
export class TokenCounterComponent implements OnInit {

  loggedIn: boolean;
  userName: string;

  constructor(private authService: MsalService) { }

  ngOnInit(): void {
    this.checkAccount();
  }

  checkAccount() {
    const account = this.authService.getAccount();
    this.loggedIn = !!account;
    if (account) {
      this.userName = account.name;
    }
  }

}
