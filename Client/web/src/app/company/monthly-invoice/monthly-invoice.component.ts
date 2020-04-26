import { Component, OnInit } from '@angular/core';
import { Customer } from '../../models/customer.model';
import { CustomerService } from '../../admin/customer/customer.service';
import { CompanyService } from '../company.service';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { DataColumn, DataColumnAction } from '../../models/dataColumn.model';
import { InvoiceService } from '../../admin/invoice/invoice.service';
import { Shipment } from '../../models/shipment.model';
import { ClassConstants } from '../../common/constants';
import { Router } from '@angular/router';
import { ToastrManager } from 'ng6-toastr-notifications';
import { AppConfigurations } from '../../config/app.config';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-monthly-invoice',
  templateUrl: './monthly-invoice.component.html',
  styleUrls: ['./monthly-invoice.component.scss']
})
export class MonthlyInvoiceListComponent implements OnInit {

  private currentlyLoggedInCompany: number = 0;
  private customerId: number = 0;

  private customers: Customer[] = [];
  private monthlyInvoices: Shipment[] = [];
  columns: DataColumn[] = [];
  printDocument: Subject<string> = new Subject<string>();

  constructor(private companyService: CompanyService, private customerService: CustomerService, private httpLoaderService: httpLoaderService,
              private invoiceService: InvoiceService, private route: Router, private toastr: ToastrManager) { 

  }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.loadAllCustomers();
    this.initializeGridColumns();
    this.loadAllMonthlyInvoices();
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Customer", value: "customerName", sortable: true, customStyling: 'column-width-200' }) );
    this.columns.push( new DataColumn({ headerText: "Slip No", value: "packingSlipNo", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Shipped Date", value: "shippingDate", sortable: true, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Shipped Via", value: "shipVia", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Update', actionStyle: ClassConstants.Warning, event: 'editInvoice' }),
      new DataColumnAction({ actionText: 'Print Invoice', actionStyle: ClassConstants.Primary, event: 'printInvoice' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deleteInvoice' })
    ] }) );
  }

  loadAllCustomers() {
    this.customerService.getAllCustomers(this.currentlyLoggedInCompany)
        .subscribe(customers => this.customers = customers.filter(c => c.invoicingtypeid == 3));
  }

  loadAllMonthlyInvoices() {
    this.httpLoaderService.show();
    this.invoiceService.getAllMonthlyInvoice(this.currentlyLoggedInCompany)
        .subscribe(
            mothlyInvoices => this.monthlyInvoices = mothlyInvoices,
            error => this.toastr.errorToastr(error.error),
            () => this.httpLoaderService.hide()
          );
  }

  addInvoice() {
    if (this.customerId > 0)
      this.route.navigateByUrl(`/companies/monthly-invoice/edit/${ this.customerId }/0/0`);
    else
      this.toastr.errorToastr('Please select the customer to create montly invoice');
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'editInvoice':
        this.route.navigateByUrl(`/companies/monthly-invoice/edit/${ data.customerId }/1/${ data.id }`);
        break;
      case 'printInvoice':
        var appConfig = new AppConfigurations();
        this.printDocument.next(`${appConfig.reportsUri}/MonthlyInvoice.aspx?id=${data.id}`);
        break;
      case 'deleteInvoice':
        if (confirm('Are you sure you want to remove this monthly invoice?')) {
          this.httpLoaderService.show();
          this.invoiceService.deleteMonthlyInvoice(data.id)
              .subscribe(
                () => {
                  this.httpLoaderService.hide();
                  this.loadAllMonthlyInvoices();
                },
                (error) => {
                  this.httpLoaderService.hide();
                  this.toastr.errorToastr(error.error);
                }
              );
        }
        break;
    }
  }
}