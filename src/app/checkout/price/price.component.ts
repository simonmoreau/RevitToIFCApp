import { Component, OnInit, Input } from '@angular/core';
import { CheckoutComponent } from '../checkout/checkout.component';

@Component({
  selector: 'app-price',
  templateUrl: './price.component.html',
  styleUrls: ['./price.component.scss']
})
export class PriceComponent implements OnInit {

  @Input() price: number;
  @Input() title: string;
  @Input() description: string;
  @Input() prodcutId: string;
  loading: boolean;

  constructor(private checkoutComponent: CheckoutComponent) { }

  ngOnInit(): void {
  }

  async checkout(event: MouseEvent ) {
    this.loading = true;
    this.checkoutComponent.checkout(event, this.prodcutId);
  }
}
