import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MonthlyInvoiceListComponent } from './monthly-invoice.component';

describe('MonthlyInvoiceComponent', () => {
  let component: MonthlyInvoiceListComponent;
  let fixture: ComponentFixture<MonthlyInvoiceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MonthlyInvoiceListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MonthlyInvoiceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
