import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../../../company/company.service';
import { InvoiceService } from '../invoice.service';
import { Invoice } from '../../../models/invoice.model';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { Supplier } from '../../../models/supplier.model';
import { SupplierService } from '../../supplier/supplier.service';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { ClassConstants } from '../../../common/constants';
import { AppConfigurations } from '../../../config/app.config';
import { ToastrManager } from 'ng6-toastr-notifications';
import { httpLoaderService } from '../../../common/services/httpLoader.service';

@Component({
  selector: 'app-invoice-list',
  templateUrl: './invoice-list.component.html',
  styleUrls: ['./invoice-list.component.scss']
})
export class InvoiceListComponent implements OnInit {

  private configuration: AppConfigurations = new AppConfigurations();
  private invoiceForm: FormGroup;
  private currentlyLoggedInCompany: number = 0;
  private appConfiguration: AppConfigurations;

  suppliers: Supplier[] = [];
  invoices: Invoice[] = [];
  filteredInvoices: Invoice[] = [];
  columns: DataColumn[] = [];

  constructor(private companyService: CompanyService, private invoiceService: InvoiceService, private supplierService: SupplierService,
              private formBuilder: FormBuilder, private router: Router, private toastr: ToastrManager, private loaderService: httpLoaderService
    ) { }

  ngOnInit() {
    this.appConfiguration = new AppConfigurations();
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.loadAllSuppliers();
    this.loadAllSupplierInvoices();
    this.invoiceForm = this.formBuilder.group({
      supplierList: FormControl,
      showNotReceivedOrders: FormControl
    });
    this.invoiceForm.get('showNotReceivedOrders').setValue(false);
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Supplier", value: "supplierName", sortable: false, minWidth: true }) );
    this.columns.push( new DataColumn({ headerText: "Invoice", value: "invoiceNo", sortable: false, minWidth: true, isLink: true }) );
    this.columns.push( new DataColumn({ headerText: "PO", value: "poNo", sortable: false, minWidth: true }) );
    this.columns.push( new DataColumn({ headerText: "Invoice Date", value: "poDate", sortable: true, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "ETA", value: "eta", sortable: true, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Rcvd", value: "isInvoiceReceived", isBoolean: true, isDisabled: true, customStyling: 'center' }) );
    this.columns.push( new DataColumn({ headerText: "Invoice", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Primary, event: 'downloadInvoice', icon: 'fa fa-download' })
    ] }) );
    this.columns.push( new DataColumn({ headerText: "Packing", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Primary, event: 'downloadPackingSlip', icon: 'fa fa-download' })
    ] }) );
    this.columns.push( new DataColumn({ headerText: "10+2", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Primary, event: 'downloadTenPlus', icon: 'fa fa-download' })
    ] }) );
    this.columns.push( new DataColumn({ headerText: "BL", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Primary, event: 'downloadBl', icon: 'fa fa-download' })
    ] }) );
    this.columns.push( new DataColumn({ headerText: "TC", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Primary, event: 'downloadTc', icon: 'fa fa-download' })
    ] }) );
    this.columns.push( new DataColumn({ headerText: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Inv', actionStyle: ClassConstants.Primary, event: 'printInvoiceBarcode', icon: 'fa fa-barcode' }),
      new DataColumnAction({ actionText: 'Box', actionStyle: ClassConstants.Primary, event: 'printBoxBarcode', icon: 'fa fa-barcode' }),
      new DataColumnAction({ actionText: 'Receive', actionStyle: ClassConstants.Primary, event: 'receiveInvoice', icon: '' }),
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Danger, event: 'deleteInvoice', icon: 'fa fa-trash' })
    ] }) );
  }

  loadAllSuppliers() {
    this.supplierService.getAllSuppliers(this.currentlyLoggedInCompany)
        .subscribe(
          (suppliers) => this.suppliers = suppliers,
          (error) => console.log(error),
          () => { console.log('Suppliers loaded'); }
        );
  }

  loadAllSupplierInvoices() {
    this.loaderService.show();
    this.invoiceService.getAllInvoices(this.currentlyLoggedInCompany)
        .subscribe(
          (invoices) => { 
            this.invoices = invoices;
            this.filteredInvoices = this.invoices;
            this.invoiceForm.get('supplierList').setValue(-1);
          },
          (error) => console.log(error),
          () => this.loaderService.hide()
        );
  }

  supplierSelected() {
    this.loaderService.show();
    var supplierId = this.invoiceForm.get('supplierList').value;
    this.invoiceService.getAllInvoices(this.currentlyLoggedInCompany)
        .subscribe((invoices) => {
          this.invoices = supplierId > 0 ? invoices.filter(s => s.supplierId == supplierId): invoices;
        }, (error) => this.toastr.errorToastr(error.error),
        () => this.loaderService.hide());
  }

  uploadInvoice() {
    var selectedSupplierId = this.invoiceForm.get('supplierList').value;
    if (selectedSupplierId > -1)
      this.router.navigateByUrl(`/invoice/upload/${selectedSupplierId}/0`);
    else
      this.toastr.warningToastr('Please select a supplier to proceed to upload');
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'printInvoiceBarcode':
        window.open(this.appConfiguration.barcodeUri + data.barcode);
        break;
      case 'printBoxBarcode':
        var boxNos = '';
        data.supplierInvoiceDetails.forEach(detail => {
          boxNos += `${ detail.barcode }|`;
        });
        window.open(this.appConfiguration.barcodeUri + boxNos);
        break;
      case 'downloadInvoice':
        if (!data.isInvoiceUploaded) {
          this.toastr.warningToastr('Document unavailable for download!!');
          return;
        }
        window.open(`${this.configuration.fileApiUri}/Invoice/${data.id}`);
        break;
      case 'downloadPackingSlip':
        if (!data.isPackingSlipUploaded) {
          this.toastr.warningToastr('Document unavailable for download!!');
          return;
        }
        window.open(`${this.configuration.fileApiUri}/PackingSlip/${data.id}`);
        break;
      case 'downloadTenPlus':
        if (!data.isTenPlusUploaded) {
          this.toastr.warningToastr('Document unavailable for download!!');
          return;
        }
        window.open(`${this.configuration.fileApiUri}/TenPlus/${data.id}`);
        break;
      case 'downloadBl':
        if (!data.isBLUploaded) {
          this.toastr.warningToastr('Document unavailable for download!!');
          return;
        }
        window.open(`${this.configuration.fileApiUri}/BL/${data.id}`);
        break;
      case 'downloadTc':
        if (!data.isTCUploaded) {
          this.toastr.warningToastr('Document unavailable for download!!');
          return;
        }
        window.open(`${this.configuration.fileApiUri}/TC/${data.id}`);
        break;
      case 'receiveInvoice':
        if (data.isInvoiceReceived) {
          this.toastr.warningToastr('This invoice has been received already');
          return;
        }
        this.invoiceService.receivedInvoice(data.supplierId, data.id)
            .subscribe(() => alert('Invoice received successfully!!'));
        break;
      case 'deleteInvoice':
        this.deleteInvoice(data);
        break;
    }
  }

  deleteInvoice(data) {
    if (data.isInvoiceReceived) {
      this.toastr.warningToastr('Received invoices cannot be removed.');
      return;
    }
    var response = confirm('Are you sure you want to remove this invoice?');
    if (response) {
      this.invoiceService.deleteInvoice(data.id)
      .subscribe(() => this.toastr.successToastr('Invoice deleted successfully'),
                 (error) => this.toastr.errorToastr(error.error));
    }
  }

  rowSelected(row: any) {
    this.router.navigateByUrl(`/invoice/upload/${row.supplierId}/1/${row.id}`);
  }

  print() {
    let printContents, popupWin;
    printContents = document.getElementById('print-section').innerHTML;
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    popupWin.document.open();
    popupWin.document.write(`
      <html>
        <head>
          <title>Print tab</title>
          <style>
          //........Customized style.......
          </style>
        </head>
    <body onload="window.print();window.close()">${printContents}</body>
      </html>`
    );
    popupWin.document.close();
  }

  showNotReceivedOrdersEvent(event) {
    if (this.invoiceForm.get('showNotReceivedOrders').value) {
      this.filteredInvoices = this.invoices.filter(i => i.isInvoiceReceived === false);
    } else {
      this.filteredInvoices = this.invoices;
    }
  }
}
