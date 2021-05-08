import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { MsalService } from '@azure/msal-angular';
import { IWorkItemStatusEntity } from '../services/api.model';

@Component({
  selector: 'app-convertions-list',
  templateUrl: './convertions-list.component.html',
  styleUrls: ['./convertions-list.component.scss']
})
export class ConvertionsListComponent implements OnInit {

  workItems: IWorkItemStatusEntity[];
  isLoading: boolean;
  zeroConversion: boolean;
  hasConversions: boolean;

  constructor(private apiService: ApiService, private authService: MsalService) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.apiService.GetUserWorkItems(this.authService.getAccount().accountIdentifier).subscribe(items =>{
      this.workItems = items;
      console.log(items);
      if (items.length == 0)
      {
        this.zeroConversion= true;
        this.hasConversions = false;
      } else {
        this.zeroConversion= false;
        this.hasConversions = true;
      }
      this.isLoading = false;
    });
  }

}
