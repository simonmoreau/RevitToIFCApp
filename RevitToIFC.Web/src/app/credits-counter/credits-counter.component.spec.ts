import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CreditsCounterComponent } from './credits-counter.component';

describe('TokenCounterComponent', () => {
  let component: CreditsCounterComponent;
  let fixture: ComponentFixture<CreditsCounterComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ CreditsCounterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreditsCounterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
