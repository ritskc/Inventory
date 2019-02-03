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
  pageSize: number = 5;
  pageNo: number = 1;
  pages: any[] = [];
  page: number = 1;

  constructor(private companyService: CompanyService, private router: Router) { 
    this.companies = [];
  }

  ngOnInit() {
    this.loadCompanies();
  }

  loadCompanies() {
    this.companyService.getAllCompanies()
      .subscribe(
        (companies) => { 
          this.companies = companies;
          this.pageNo = Math.ceil(this.companies.length / this.pageSize);
          this.createRange();
        },
        (error) => { console.log(error); }
    );
  }

  addCompany() {
    this.router.navigateByUrl(`/companies/detail/${ UserAction.Add }/#`);
  }

  companySelected(id: number) {
    this.router.navigateByUrl(`/companies/detail/${ UserAction.Edit }/${id}`);
  }

  createRange(){
    for(var i = 1; i <= this.pageNo; i++){
       this.pages.push(i);
    }
  }

  setPageNo(item: number) {
    this.page = item;
  }
}
