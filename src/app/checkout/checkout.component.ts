import { Component, HostListener, Input, OnInit } from '@angular/core';
import { MsalService } from '@azure/msal-angular';
import { environment } from 'src/environments/environment';

declare var StripeCheckout: StripeCheckoutStatic;

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements OnInit {

  constructor(private authService: MsalService) { }

  @Input() amount;
  @Input() description;

  handler: StripeCheckoutHandler;

  confirmation: any;
  loading = false;

  ngOnInit(): void {
    this.handler = StripeCheckout.configure({
      key: environment.stripe_key,
      image: 'https://www.bim42.com/assets/BIM42_Logo_Embossed_7_FondNoir.png',
      locale: 'auto',
      source: async (source) => {
        this.loading = true;
        const account = await this.authService.getAccount();
        // const fun = this.functions.httpsCallable('stripeCreateCharge');
        // this.confirmation = await fun({ source: source.id, uid: account.sid, amount: this.amount }).toPromise();
        this.loading = false;
      }
    });
  }

    // Open the checkout handler
    async checkout(e) {
      const account = await this.authService.getAccount();
      this.handler.open({
        name: 'Fireship Store',
        description: this.description,
        amount: this.amount,
        email: account.userName,
      });
      e.preventDefault();
    }
  
    // Close on navigate
    @HostListener('window:popstate')
    onPopstate() {
      this.handler.close();
    }

}
