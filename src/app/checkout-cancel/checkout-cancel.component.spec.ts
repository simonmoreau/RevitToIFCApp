import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckoutCancelComponent } from './checkout-cancel.component';

describe('CheckoutCancelComponent', () => {
  let component: CheckoutCancelComponent;
  let fixture: ComponentFixture<CheckoutCancelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CheckoutCancelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CheckoutCancelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
