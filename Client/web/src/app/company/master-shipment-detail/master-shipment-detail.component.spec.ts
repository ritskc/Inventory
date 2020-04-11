import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MasterShipmentDetailComponent } from './master-shipment-detail.component';

describe('MasterShipmentDetailComponent', () => {
  let component: MasterShipmentDetailComponent;
  let fixture: ComponentFixture<MasterShipmentDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MasterShipmentDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MasterShipmentDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
