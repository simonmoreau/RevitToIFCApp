import { TestBed } from '@angular/core/testing';

import { CreditsCounterService } from './credits-counter.service';

describe('CreditsCounterService', () => {
  let service: CreditsCounterService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CreditsCounterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
