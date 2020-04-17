import { Component, OnInit } from '@angular/core';
import { CustomerService } from '../customer.service';
import { CompanyService } from '../../../company/company.service';
import { Router } from '@angular/router';
import { Customer } from '../../../models/customer.model';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { UserAction } from '../../../models/enum/userAction';
import { ClassConstants } from '../../../common/constants';
import { ToastrManager } from 'ng6-toastr-notifications';

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
              private loaderService: httpLoaderService, private router: Router, private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.getAllCustomers();
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Name", value: "name", sortable: true, customStyling: 'column-width-200' }) );
    this.columns.push( new DataColumn({ headerText: "Address", value: "addressLine1", sortable: true, customStyling: 'column-width-200' }) );
    this.columns.push( new DataColumn({ headerText: "Phone No", value: "telephoneNumber", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Email", value: "emailAddress", sortable: true, customStyling: 'column-width-200' }) );
    this.columns.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, actions: [
      new DataColumnAction({ actionText: 'Update', actionStyle: ClassConstants.Warning, event: 'editCustomer' }),
      new DataColumnAction({ actionText: 'Orders', actionStyle: ClassConstants.Primary, event: 'managePurchaseOrder' }),
      new DataColumnAction({ actionText: 'Shipment', actionStyle: ClassConstants.Primary, event: 'manageShipment' }),
      new DataColumnAction({ actionText: 'Invoice', actionStyle: ClassConstants.Primary, event: 'manageInvoice' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deleteCustomer' })
    ] }) );
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

  deleteCustomer(row) {
    if (confirm('Are you sure you want to remove this customer?')) {
      this.service.delete(row.id)
          .subscribe(() => this.toastr.successToastr('Customer removed successfully'));
    }
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'editCustomer':
        this.rowSelected(data);
        break;
      case 'managePurchaseOrder':
        this.redirectToCustomerPurchaseOrder(data);
        break;
      case 'manageShipment':
        this.redirectToCreateShipment(data);
        break;
      case 'manageInvoice':
        this.redirectoToCustomerInvoice(data);
        break;
      case 'deleteCustomer':
        this.deleteCustomer(data);
        break;
    }
  }

  redirectToCustomerPurchaseOrder(customer: Customer) {
    this.router.navigateByUrl(`/customers/purchase-order/${ customer.id }/${ UserAction.Details }`);
  }

  redirectToCreateShipment(customer: Customer) {
    this.router.navigateByUrl(`/companies/create-shipment/${ customer.id }`);
  }

  redirectoToCustomerInvoice(customer: Customer) {
    this.router.navigateByUrl(`/companies/invoice`);
  }
}
