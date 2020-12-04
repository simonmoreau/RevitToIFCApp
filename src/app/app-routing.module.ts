import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';
import { HomeComponent } from './home/home.component';
import { ProfileComponent } from './profile/profile.component';
import { UploadComponent } from './upload/upload.component';
import { CheckoutComponent } from './checkout/checkout/checkout.component';
import { CheckoutCancelComponent } from './checkout/checkout-cancel/checkout-cancel.component';
import { CheckoutSuccessComponent } from './checkout/checkout-success/checkout-success.component';

const routes: Routes = [
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [
      MsalGuard
    ]
  },
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'upload',
    component: UploadComponent,
    canActivate: [
      MsalGuard
    ]
  }
  ,
  {
    path: 'checkout',
    component: CheckoutComponent,
    canActivate: [
      MsalGuard
    ]
  }
  ,
  {
    path: 'checkout/success',
    component: CheckoutSuccessComponent,
    canActivate: [
      MsalGuard
    ]
  }
  ,
  {
    path: 'checkout/cancel',
    component: CheckoutCancelComponent,
    canActivate: [
      MsalGuard
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: false })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
