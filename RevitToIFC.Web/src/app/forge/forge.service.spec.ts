import { TestBed } from '@angular/core/testing';

import { ForgeService } from './forge.service';

describe('ForgeService', () => {
  let service: ForgeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ForgeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
