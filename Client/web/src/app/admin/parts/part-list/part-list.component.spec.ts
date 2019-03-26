import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartListComponent } from './part-list.component';

describe('PartListComponent', () => {
  let component: PartListComponent;
  let fixture: ComponentFixture<PartListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
