import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckoutSuccessComponent } from './checkout-success.component';

describe('CheckoutSuccessComponent', () => {
  let component: CheckoutSuccessComponent;
  let fixture: ComponentFixture<CheckoutSuccessComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CheckoutSuccessComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CheckoutSuccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
