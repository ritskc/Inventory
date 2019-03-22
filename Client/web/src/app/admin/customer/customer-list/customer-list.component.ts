import { Component, OnInit } from '@angular/core';
import { CustomerService } from '../customer.service';
import { CompanyService } from '../../../company/company.service';
import { Router } from '@angular/router';
import { Customer } from '../../../models/customer.model';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { DataColumn } from '../../../models/dataColumn.model';
import { UserAction } from '../../../models/enum/userAction';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss']
})
export class CustomerListComponent implements OnInit {

  customers: Customer[] = [];
  columns: DataColumn[] = [];
  currentlyLoggedInCompanyId: number = 0;

  constructor(private service: CustomerService, private companyService: CompanyService, 
              private loaderService: httpLoaderService, private router: Router) { }

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
        );
  }

  rowSelected(row) {
    this.router.navigateByUrl(`/customers/detail/${ UserAction.Edit }/${row.id}`);
  }

  addCustomer() {
    this.router.navigateByUrl(`/customers/detail/${ UserAction.Add }/#`);
  }
}
