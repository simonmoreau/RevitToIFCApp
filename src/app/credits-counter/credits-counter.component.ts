import { Component, Input, OnInit } from '@angular/core';
import { CreditsCounterService} from './credits-counter.service';

@Component({
  selector: 'app-credits-counter',
  templateUrl: './credits-counter.component.html',
  styleUrls: ['./credits-counter.component.scss']
})
export class CreditsCounterComponent implements OnInit {

  displayedCredits: number;
  zeroCredits: boolean;
  oneCredits: boolean;
  moreCredits: boolean;
  isLoading: boolean;
  creditsCounterService: CreditsCounterService;

  constructor(private creditsService: CreditsCounterService) {
    this.creditsCounterService = creditsService;

    this.creditsCounterService.displayedCreditsEvent.subscribe(c => {
      this.displayedCredits = c;
      this.updateVisibility(this.displayedCredits);
    });
   }

  ngOnInit(): void {
    this.isLoading = true;
    this.creditsCounterService.GetConversionCredits().subscribe(c => {
      this.displayedCredits = c.creditsNumber;
      this.updateVisibility(this.displayedCredits);
      this.isLoading = false;
    });
  }



  updateVisibility(creditsNumber: number): void {
    if (!creditsNumber)
    {
      this.displayedCredits = 0;
      this.zeroCredits = true;
      this.oneCredits = false;
      this.moreCredits = false;
    }
    else if (creditsNumber == 0)
    {
      this.zeroCredits = true;
      this.oneCredits = false;
      this.moreCredits = false;
    }
    else if (creditsNumber == 1)
    {
      this.zeroCredits = false;
      this.oneCredits = true;
      this.moreCredits = false;
    }
    else
    {
      this.zeroCredits = false;
      this.oneCredits = false;
      this.moreCredits = true;
    }
  }

}
