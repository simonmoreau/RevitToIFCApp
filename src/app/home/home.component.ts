import { Component, OnInit } from '@angular/core';
import { MsalService } from '@azure/msal-angular';

import { ForgeService } from '../forge/forge.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})


export class HomeComponent implements OnInit {

  profile: any;
  callContent: string;
  
  constructor(private authService: MsalService, private forgeService: ForgeService) { }

  ngOnInit() {

    this.profile = this.authService.getAccount();
  }

  onForgeCall(){
    this.forgeService.getActivities().subscribe(a => this.callContent = JSON.stringify(a));
  }
}
