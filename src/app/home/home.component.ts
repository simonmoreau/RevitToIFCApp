import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MsalService } from '@azure/msal-angular';

import { ApiService } from '../services/api.service';

import { ForgeService } from '../forge/forge.service';
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  profile: any;
  callContent: string;
  rootURL = '/api';

  constructor(
    private authService: MsalService,
    private apiService: ApiService,
    private http: HttpClient,
    iconRegistry: MatIconRegistry,
    sanitizer: DomSanitizer
  ) {
    iconRegistry.addSvgIcon(
      'thumbs-up',
      sanitizer.bypassSecurityTrustResourceUrl(
        'assets/img/examples/thumbup-icon.svg'
      )
    );
  }

  ngOnInit() {
    this.profile = this.authService.getAccount();
  }

  onForgeCall() {
    // this.forgeService.getActivities().subscribe(a => this.callContent = JSON.stringify(a));

    this.apiService.getLocalAPI().subscribe((a) => (this.callContent = a.name));

    // this.getTasks().subscribe(r => console.log(r));
  }

  getTasks() {
    return this.http.get(this.rootURL + '/message');
  }
}
