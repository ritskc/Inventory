import { Component, OnInit } from '@angular/core';
import { Company } from '../../models/company.model';
import { CompanyService } from '../company.service';
import { Router } from '@angular/router';
import { UserAction } from '../../models/enum/userAction';

@Component({
  selector: 'app-company-list',
  templateUrl: './company-list.component.html',
  styleUrls: ['./company-list.component.scss']
})
export class CompanyListComponent implements OnInit {

  companies: Company[];

  constructor(private companyService: CompanyService, private router: Router) { 
    this.companies = [];
  }

  ngOnInit() {
    this.loadCompanies();
  }

  loadCompanies() {
    this.companyService.getAllCompanies()
      .subscribe(
        (companies) => { this.companies = companies; },
        (error) => { console.log(error); }
    );
  }

  addCompany() {
    this.router.navigateByUrl(`/companies/detail/${ UserAction.Add }/#`);
  }

  companySelected(id: number) {
    this.router.navigateByUrl(`/companies/detail/${ UserAction.Edit }/${id}`);
  }
}
