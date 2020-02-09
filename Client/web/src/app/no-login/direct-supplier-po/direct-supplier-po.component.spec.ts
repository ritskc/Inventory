import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DirectSupplierPoComponent } from './direct-supplier-po.component';

describe('DirectSupplierPoComponent', () => {
  let component: DirectSupplierPoComponent;
  let fixture: ComponentFixture<DirectSupplierPoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DirectSupplierPoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DirectSupplierPoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
