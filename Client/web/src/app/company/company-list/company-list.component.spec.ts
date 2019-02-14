import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CompanyListComponent } from './company-list.component';
import { CompanyService } from '../company.service';
import { ApiService } from '../../common/services/api.service';
import { HttpClientModule } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { Company } from '../../models/company.model';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

describe('CompanyListComponent', () => {
  let component: CompanyListComponent;
  let fixture: ComponentFixture<CompanyListComponent>;
  let apiService: ApiService;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ 
        CompanyListComponent 
      ],
      imports: [
        HttpClientModule,
        HttpClientTestingModule
      ],
      providers: [
        ApiService,
        CompanyService,
        { provide: Router, useValue: { }}
      ],
      schemas: [
        CUSTOM_ELEMENTS_SCHEMA
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CompanyListComponent);
    component = fixture.componentInstance;
    apiService = TestBed.get(ApiService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should list all available companies', () => {
    spyOn(apiService, 'get').and.returnValue(CompanyStub.getAllCompanies());
  });
});

export class CompanyStub {
  static getAllCompanies(): Company[] {
    var companies: Company[] = [];

    var company1 = new Company({id: 1, name: 'Harisons Industrial Products Inc', address: '731 GROVE AVENUE, EDISON, NJ 08820', phoneNo: '(732) 321 5076', faxNo: '(732) 321 3369', eMail: 'web@harisons.com', contactPersonName: 'Jigen Shah'});
    companies.push(company1);

    return companies;
  }
}