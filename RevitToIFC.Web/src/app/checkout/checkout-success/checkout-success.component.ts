import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap, Params } from '@angular/router';

import { MsalService } from '@azure/msal-angular';
import { Observable } from 'rxjs';
import { flatMap } from 'rxjs/operators';
import { CreditsCounterService } from 'src/app/credits-counter/credits-counter.service';
import { environment } from 'src/environments/environment';
import { IConversionCreditsUpdate } from '../../services/api.model';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-checkout-success',
  templateUrl: './checkout-success.component.html',
  styleUrls: ['./checkout-success.component.scss'],
})
export class CheckoutSuccessComponent implements OnInit {

  isProcessing = true;
  isSuccess = false;
  isError = false;
  conversionCreditsUpdate: IConversionCreditsUpdate;

  constructor(private route: ActivatedRoute, private creditsCounterService: CreditsCounterService) {
    this.isProcessing = true;
    this.isSuccess = false;
    this.isError = false;
    this.conversionCreditsUpdate = {
      userId: '',
      creditsNumber: 0
    };
  }

  ngOnInit(): void {

    this.isProcessing = true;
    this.isSuccess = false;
    this.isError = false;

    this.route.queryParams.pipe(
      flatMap((params: Params) => this.creditsCounterService.UpdateConversionCredits(params['session_id']))
    ).subscribe(
      (result: IConversionCreditsUpdate) => {
        this.conversionCreditsUpdate = result;
        this.isProcessing = false;
        this.isSuccess = true;
        this.isError = false;
        console.log(result);
      },
      (error) => {
        this.isProcessing = false;
        this.isSuccess = false;
        this.isError = true;
        console.log(error);
       }
      );
  }
}
