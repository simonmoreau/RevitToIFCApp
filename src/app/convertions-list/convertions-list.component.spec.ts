import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConvertionsListComponent } from './convertions-list.component';

describe('ConvertionsListComponent', () => {
  let component: ConvertionsListComponent;
  let fixture: ComponentFixture<ConvertionsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConvertionsListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConvertionsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
