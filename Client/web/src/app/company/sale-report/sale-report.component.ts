import { Component, OnInit } from '@angular/core';
import { Customer } from '../../models/customer.model';
import { DataColumn } from '../../models/dataColumn.model';
import { CompanyService } from '../company.service';
import { CustomerService } from '../../admin/customer/customer.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { SupplierService } from '../../admin/supplier/supplier.service';
import { Supplier } from '../../models/supplier.model';

@Component({
  selector: 'app-sale-report',
  templateUrl: './sale-report.component.html',
  styleUrls: ['./sale-report.component.scss']
})
export class SaleReportComponent implements OnInit {

  private customerId: number = -1;
  private supplierId: number = -1;
  private unfileterdData: any[] = [];
  private saleReport: any[] = [];
  private currentlyLoggedInCompany: number = 0;
  private customers: Customer[] = [];
  private suppliers: Supplier[] = [];
  private columns: DataColumn[] = [];
  private from: Date;
  private to: Date;
  private showSummary: boolean = false;

  constructor(private companyService: CompanyService, private customerService: CustomerService, private supplierService: SupplierService,
              private loaderService: httpLoaderService, private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.loadAllCustomer();
    this.loadAllSuppliers();
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

  loadAllSuppliers() {
    this.loaderService.show();
    this.supplierService.getAllSuppliers(this.currentlyLoggedInCompany)
        .subscribe(suppliers => this.suppliers = suppliers,
          (error) => this.toastr.errorToastr(error.error),
          () => this.loaderService.hide());
  }

  initializeGrid() {
    this.columns = [];
    this.columns.push( new DataColumn({ headerText: "Customer", value: "customerName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Supplier", value: "supplierName", sortable: true }) );
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
    this.customerService.getSaleReport(this.currentlyLoggedInCompany, this.from.toString(), this.to.toString(), this.showSummary)
        .subscribe((result) => {
          this.unfileterdData = result;
          this.fileterSaleReportForCustomerSelection();
        }, 
        (error) => this.toastr.errorToastr(error.error),
        () => this.loaderService.hide());
  }

  supplierSelected() {
    this.loaderService.show();
    this.fileterSaleReportForCustomerSelection();
    this.loaderService.hide();
  }

  customerSelected() {
    this.fileterSaleReportForCustomerSelection();
  }

  private fileterSaleReportForCustomerSelection() {
    if (this.supplierId > 0 || this.customerId > 0) {
      if (this.supplierId > 0 && this.customerId < 1) {
        var supplierName = this.suppliers.find(s => s.id == this.supplierId).name;
        this.saleReport = this.unfileterdData.filter(s => s.supplierName == supplierName);
      }
      else if (this.supplierId < 1 && this.customerId > 0) {
        var customerName = this.customers.find(c => c.id == this.customerId).name;
        this.saleReport = this.unfileterdData.filter(s => s.customerName == customerName);
      }
      else if (this.supplierId > 0 && this.customerId > 0) {
        var supplierName = this.suppliers.find(s => s.id == this.supplierId).name;
        var customerName = this.customers.find(c => c.id == this.customerId).name;
        this.saleReport = this.unfileterdData.filter(s => s.customerName == customerName && s.supplierName == supplierName);
      }
    } else {
      this.saleReport = this.unfileterdData;
    }
  }
}
