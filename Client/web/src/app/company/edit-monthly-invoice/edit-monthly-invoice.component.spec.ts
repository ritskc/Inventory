import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditMonthlyInvoiceComponent } from './edit-monthly-invoice.component';

describe('EditMonthlyInvoiceComponent', () => {
  let component: EditMonthlyInvoiceComponent;
  let fixture: ComponentFixture<EditMonthlyInvoiceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditMonthlyInvoiceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditMonthlyInvoiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
