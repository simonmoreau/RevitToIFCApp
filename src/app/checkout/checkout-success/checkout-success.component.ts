import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap, Params } from '@angular/router';

import { MsalService } from '@azure/msal-angular';
import { Observable } from 'rxjs';
import { flatMap } from 'rxjs/operators';
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

  constructor(private route: ActivatedRoute, private authService: MsalService, private apiService: ApiService) {
    this.isProcessing = true;
    this.isSuccess = false;
    this.isError = false;
  }

  ngOnInit(): void {

    this.isProcessing = true;
    this.isSuccess = false;
    this.isError = false;
    const userId = this.authService.getAccount().accountIdentifier;

    const updateConversionTokens = (params: Params): Observable<IConversionCreditsUpdate> => {
      const checkoutSessionId = params['session_id'];
      return this.apiService.updateConversionCredits(
        userId,
        checkoutSessionId
      );
    };

    this.route.queryParams.pipe(
      flatMap((params: Params) => updateConversionTokens(params))
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
