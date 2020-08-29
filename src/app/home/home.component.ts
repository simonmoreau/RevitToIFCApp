import { Component, OnInit } from '@angular/core';
import { MsalService } from '@azure/msal-angular';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})


export class HomeComponent implements OnInit {

  profile: any;
  
  constructor(private authService: MsalService) { }

  ngOnInit() {

    this.profile = this.authService.getAccount();
  }
}
