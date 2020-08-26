import { Component, OnInit } from '@angular/core';
import { Supplier } from '../../../models/supplier.model';
import { DataColumn } from '../../../models/dataColumn.model';
import { CompanyService } from '../../../company/company.service';
import { SupplierService } from '../supplier.service';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { CustomerService } from '../../customer/customer.service';
import { Customer } from '../../../models/customer.model';

@Component({
  selector: 'app-purchase-report',
  templateUrl: './purchase-report.component.html',
  styleUrls: ['./purchase-report.component.scss']
})
export class PurchaseReportComponent implements OnInit {

  private supplierId: number = -1;
  private customerId: number = -1;
  private unfilteredData: any[] = [];
  private purchaseReport: any[] = [];
  private currentlyLoggedInCompany: number = 0;
  private suppliers: Supplier[] = [];
  private customers: Customer[] = [];
  private columns: DataColumn[] = [];
  private from: Date;
  private to: Date;
  private showSummary: boolean = false;

  constructor(private companyService: CompanyService, private customerService: CustomerService, private supplierService: SupplierService, 
    private loaderService: httpLoaderService, private toastr: ToastrManager
    ) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.loadAllCustomers();
    this.loadAllSuppliers();
    this.initializeGrid();
  }

  loadAllSuppliers() {
    this.loaderService.show();
    this.supplierService.getAllSuppliers(this.currentlyLoggedInCompany)
        .subscribe(suppliers => this.suppliers = suppliers,
          (error) => this.toastr.errorToastr(error.error),
          () => this.loaderService.hide());
  }

  loadAllCustomers() {
    this.loaderService.show();
    this.customerService.getAllCustomers(this.currentlyLoggedInCompany)
        .subscribe(
          customers => this.customers = customers,
          (error) => this.toastr.errorToastr(error.error),
          () => this.loaderService.hide()
        );
  }

  initializeGrid() {
    this.columns = [];
    this.columns.push( new DataColumn({ headerText: "Supplier", value: "supplierName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Customer", value: "customerName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Invoice No", value: "invoiceNo", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Invoice Date", value: "invoiceDate", sortable: true, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Received Date", value: "receivedDate", sortable: true, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Code", value: "code", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "description", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Qty", value: "qty", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "In Transit", value: "inTransitQty", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Price", value: "price", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Total", value: "total", customStyling: 'right' }) );
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
    this.supplierService.getPurchaseReport(this.currentlyLoggedInCompany, this.from.toString(), this.to.toString(), this.showSummary)
        .subscribe((result) => {
          this.unfilteredData = result;
          this.filterPurchaseReportForSupplierSelection();
        }, 
        (error) => this.toastr.errorToastr(error.error),
        () => this.loaderService.hide());
  }

  supplierSelected() {
    this.loaderService.show();
    this.filterPurchaseReportForSupplierSelection();
    this.loaderService.hide();
  }

  customerSelected() {
    this.loaderService.show();
    this.filterPurchaseReportForSupplierSelection();
    this.loaderService.hide();
  }

  private filterPurchaseReportForSupplierSelection() {
    if (this.supplierId > 0 || this.customerId > 0) {
      if (this.supplierId > 0 && this.customerId < 1) {
        var supplierName = this.suppliers.find(s => s.id == this.supplierId).name;
        this.purchaseReport = this.unfilteredData.filter(s => s.supplierName == supplierName);
      }
      else if (this.supplierId < 1 && this.customerId > 0) {
        var customerName = this.customers.find(c => c.id == this.customerId).name;
        this.purchaseReport = this.unfilteredData.filter(s => s.customerName == customerName);
      }
      else if (this.supplierId > 0 && this.customerId > 0) {
        var supplierName = this.suppliers.find(s => s.id == this.supplierId).name;
        var customerName = this.customers.find(c => c.id == this.customerId).name;
        this.purchaseReport = this.unfilteredData.filter(s => s.customerName == customerName && s.supplierName == supplierName);
      }
    } else {
      this.purchaseReport = this.unfilteredData;
    }
  }
}
