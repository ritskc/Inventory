import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SimpleGridComponent } from './simple-grid.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { PaginatePipe } from '../../pipes/paginate.pipe';
import { SearchPipe } from '../../pipes/search.pipe';

describe('SimpleGridComponent', () => {
  let component: SimpleGridComponent;
  let fixture: ComponentFixture<SimpleGridComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        PaginatePipe,
        SearchPipe,
        SimpleGridComponent
      ],
      schemas: [
        NO_ERRORS_SCHEMA
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SimpleGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
