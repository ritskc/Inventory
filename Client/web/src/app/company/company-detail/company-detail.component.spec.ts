import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CompanyDetailComponent } from './company-detail.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { HttpClientModule } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ToastrManager } from 'ng6-toastr-notifications';
import { ToastrOptions } from 'ng6-toastr-notifications/lib/toastr.options';

describe('CompanyDetailComponent', () => {
  let component: CompanyDetailComponent;
  let fixture: ComponentFixture<CompanyDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ 
        CompanyDetailComponent
      ],
      imports: [
        HttpClientModule,
        HttpClientTestingModule
      ],
      providers: [
        FormBuilder,
        ToastrManager,
        { provide: ActivatedRoute, useValue: {} },
        { provide: ToastrOptions, useValue: ToastrOptionsReplacement }
      ],
      schemas: [
        NO_ERRORS_SCHEMA
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CompanyDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

export declare class ToastrOptionsReplacement {
  position: string;
  maxShown: number;
  newestOnTop: boolean;
  animate: string;
  toastTimeout: number;
  enableHTML: boolean;
  dismiss: string;
  messageClass: string;
  titleClass: string;
  showCloseButton: boolean;
  constructor();
}