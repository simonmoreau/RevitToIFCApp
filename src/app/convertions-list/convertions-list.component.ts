import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

import { ApiService } from '../services/api.service';
import { MsalService } from '@azure/msal-angular';
import { IWorkItemStatusEntity } from '../services/api.model';

export interface WorkItemData {
  status: string;
  name: string;
  fileUrl: string;
  report: string;
}

@Component({
  selector: 'app-convertions-list',
  templateUrl: './convertions-list.component.html',
  styleUrls: ['./convertions-list.component.scss'],
})
export class ConvertionsListComponent implements OnInit {
  workItems: IWorkItemStatusEntity[];
  isLoading: boolean;
  zeroConversion: boolean;
  hasConversions: boolean;

  displayedColumns: string[] = ['name', 'status', 'fileUrl', 'report'];
  dataSource: MatTableDataSource<WorkItemData>;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private apiService: ApiService, private authService: MsalService ) {  }

  ngOnInit(): void {
    this.isLoading = true;
    this.apiService
      .GetUserWorkItems(this.authService.getAccount().accountIdentifier)
      .subscribe((items) => {
        this.workItems = items;
        const workItemDataArray = items.map(item => this.CreateNewWorkItemData(item));

        // Assign the data to the data source for the table to render
    this.dataSource = new MatTableDataSource(workItemDataArray);

    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    
        console.log(items);
        if (items.length == 0) {
          this.zeroConversion = true;
          this.hasConversions = false;
        } else {
          this.zeroConversion = false;
          this.hasConversions = true;
        }
        this.isLoading = false;
      });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  /** Builds and returns a new User. */
CreateNewWorkItemData(item: IWorkItemStatusEntity): WorkItemData {

  return {
    status: item.status,
    name: item.fileName,
    fileUrl: item.fileUrl,
    report: item.reportUrl
  };
}
}
