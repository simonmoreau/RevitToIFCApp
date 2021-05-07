import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { MatToolbarModule } from '@angular/material/toolbar';
import {MatIconModule} from '@angular/material/icon';
import {MatTooltipModule} from '@angular/material/tooltip';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {MatGridListModule} from '@angular/material/grid-list';
import {MatProgressBarModule} from '@angular/material/progress-bar';
import {MatExpansionModule} from '@angular/material/expansion';

import { FlexLayoutModule } from '@angular/flex-layout';
import { NgcCookieConsentModule, NgcCookieConsentConfig } from 'ngx-cookieconsent';

import { Configuration } from 'msal';
import {
  MsalModule,
  MsalInterceptor,
  MSAL_CONFIG,
  MSAL_CONFIG_ANGULAR,
  MsalService,
  MsalAngularConfiguration
} from '@azure/msal-angular';
import { FileUploadModule} from './file-upload/file-upload.module';

import { msalConfig, msalAngularConfig } from './app-config';
import { AppRoutingModule } from './app-routing.module';

// Components
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { ProfileComponent } from './profile/profile.component';
import { UploadComponent } from './upload/upload.component';

// Pipes

// Services
import { InterceptorService } from './services/interceptor.service';
import { from } from 'rxjs';
import { CheckoutComponent } from './checkout/checkout/checkout.component';
import { CheckoutSuccessComponent } from './checkout/checkout-success/checkout-success.component';
import { CheckoutCancelComponent } from './checkout/checkout-cancel/checkout-cancel.component';
import { PriceComponent } from './checkout/price/price.component';
import { CreditsCounterComponent } from './credits-counter/credits-counter.component';
import { environment } from '../environments/environment';
import { ConvertionsListComponent } from './convertions-list/convertions-list.component';


function MSALConfigFactory(): Configuration {
  return msalConfig;
}

function MSALAngularConfigFactory(): MsalAngularConfiguration {
  return msalAngularConfig;
}

const cookieConfig:NgcCookieConsentConfig = {
  cookie: {
    domain: environment.localDomain // or 'your.domain.com' // it is mandatory to set a domain, for cookies to work properly (see https://goo.gl/S2Hy2A)
  },
  palette: {
    popup: {
      background: '#1c364a',
      text: "#fff"
    },
    button: {
      background: '#e74727',
      text: "#fff"
    }
  },
  position: 'bottom-right',
  theme: 'classic',
  type: 'info'
};

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ProfileComponent,
    UploadComponent,
    CheckoutComponent,
    CheckoutSuccessComponent,
    CheckoutCancelComponent,
    PriceComponent,
    CreditsCounterComponent,
    ConvertionsListComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    MatToolbarModule,
    MatButtonModule,
    MatListModule,
    AppRoutingModule,
    MatCardModule,
    MatIconModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    MatProgressBarModule,
    MatGridListModule,
    MatExpansionModule,
    FlexLayoutModule,
    MsalModule,
    FileUploadModule,
    NgcCookieConsentModule.forRoot(cookieConfig)
  ],
  providers: [
    MsalService,
    {
      provide: MSAL_CONFIG,
      useFactory: MSALConfigFactory
    },
    {
      provide: MSAL_CONFIG_ANGULAR,
      useFactory: MSALAngularConfigFactory
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: InterceptorService,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
