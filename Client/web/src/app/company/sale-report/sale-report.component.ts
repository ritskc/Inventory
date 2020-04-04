import { Component, OnInit } from '@angular/core';
import { Customer } from '../../models/customer.model';
import { DataColumn } from '../../models/dataColumn.model';
import { CompanyService } from '../company.service';
import { CustomerService } from '../../admin/customer/customer.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { httpLoaderService } from '../../common/services/httpLoader.service';

@Component({
  selector: 'app-sale-report',
  templateUrl: './sale-report.component.html',
  styleUrls: ['./sale-report.component.scss']
})
export class SaleReportComponent implements OnInit {

  private customerId: number = -1;
  private unfileterdData: any[] = [];
  private saleReport: any[] = [];
  private currentlyLoggedInCompany: number = 0;
  private customers: Customer[] = [];
  private columns: DataColumn[] = [];
  private from: Date;
  private to: Date;

  constructor(private companyService: CompanyService, private customerService: CustomerService, private loaderService: httpLoaderService,
              private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.loadAllCustomer();
    this.initializeGrid();
  }

  loadAllCustomer() {
    this.loaderService.show();
    this.customerService.getAllCustomers(this.currentlyLoggedInCompany)
        .subscribe(
          (customers) => this.customers = customers,
          (error) => this.toastr.errorToastr(error.error),
          () => this.loaderService.hide()
        );
  }

  initializeGrid() {
    this.columns = [];
    this.columns.push( new DataColumn({ headerText: "Customer", value: "customerName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Code", value: "code", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "description", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Packing Slip No", value: "packingSlipNo", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Shipping Date", value: "shippingDate", sortable: true, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Qty", value: "qty", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Unit Price", value: "unitPrice", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Price", value: "price", customStyling: 'right' }) );
  }

  generate() {
    if (!this.from || !this.to) {
      this.toastr.errorToastr('Please select a valid dates');
      return;
    }

    if (this.to < this.from) {
      this.toastr.errorToastr('To date cannot be before From date');
      return;
    }

    this.loaderService.show();
    this.customerService.getSaleReport(this.currentlyLoggedInCompany, this.from.toString(), this.to.toString())
        .subscribe((result) => {
          this.unfileterdData = result;
          this.fileterSaleReportForCustomerSelection();
        }, 
        (error) => this.toastr.errorToastr(error.error),
        () => this.loaderService.hide());
  }

  customerSelected() {
    this.fileterSaleReportForCustomerSelection();
  }

  private fileterSaleReportForCustomerSelection() {
    if (this.customerId > 0) {
      var customerName = this.customers.find(c => c.id == this.customerId).name;
      this.saleReport = this.unfileterdData.filter(c => c.customerName == customerName);
    } else {
      this.saleReport = this.unfileterdData;
    }

  }
}
