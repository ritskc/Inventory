import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadInvoiceComponent } from './upload-invoice.component';

describe('UploadInvoiceComponent', () => {
  let component: UploadInvoiceComponent;
  let fixture: ComponentFixture<UploadInvoiceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UploadInvoiceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadInvoiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
