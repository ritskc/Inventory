import { Component, OnInit } from '@angular/core';
import { CustomerService } from '../customer.service';
import { Customer } from '../../../models/customer.model';
import { DataColumn } from '../../../models/dataColumn.model';
import { CompanyService } from '../../../company/company.service';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss']
})
export class CustomerListComponent implements OnInit {

  currentlyLoggedInCompanyId: number;
  customers: Customer[];
  columns: DataColumn[] = [];

  constructor(private service: CustomerService, private companyService: CompanyService,
              private loaderService: httpLoaderService, private router: Router) { 
    this.customers = [];
  }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.getAllCustomers();
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Name", value: "name", isLink: true, sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Address", value: "addressLine1", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Phone No", value: "telephoneNumber", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Email", value: "emailAddress", sortable: true }) );
  }

  getAllCustomers() {
    this.loaderService.show();
    this.service.getAllCustomers(this.currentlyLoggedInCompanyId)
      .subscribe(
        (customers) => {
          this.customers = customers;
          this.loaderService.hide();
        },
        (error) => {
          console.log(error);
          this.loaderService.hide();
        }
      )
    
  }
}
