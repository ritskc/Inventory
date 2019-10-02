import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InventoryPartsListComponent } from './inventory-parts-list.component';

describe('InventoryPartsListComponent', () => {
  let component: InventoryPartsListComponent;
  let fixture: ComponentFixture<InventoryPartsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InventoryPartsListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InventoryPartsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
