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
import { map, mergeMap } from 'rxjs/operators';
import { FileUploadService } from '../../../common/services/file-upload.service';

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
  private fileAttributes: any;
  private dataToUpdate: any;

  suppliers: Supplier[] = [];
  invoices: Invoice[] = [];
  filteredInvoices: any[] = [];
  columns: DataColumn[] = [];

  constructor(private companyService: CompanyService, private invoiceService: InvoiceService, private supplierService: SupplierService,
              private formBuilder: FormBuilder, private router: Router, private toastr: ToastrManager, 
              private loaderService: httpLoaderService, private fileUploadService: FileUploadService
    ) { }

  ngOnInit() {
    this.appConfiguration = new AppConfigurations();
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.invoiceForm = this.formBuilder.group({
      supplierList: FormControl,
      showNotReceivedOrders: FormControl,
      showDetails: FormControl
    });
    this.invoiceForm.get('showNotReceivedOrders').setValue(false);
    this.invoiceForm.get('showDetails').setValue(false);

    this.initializeGridColumns();
    this.loadAllSuppliers();
  }

  initializeGridColumns() {
    this.columns = [];
    if (!this.invoiceForm.get('showDetails').value) {
      this.columns.push( new DataColumn({ headerText: "Supplier", value: "supplierName", sortable: false, minWidth: true }) );
      this.columns.push( new DataColumn({ headerText: "Invoice", value: "invoiceNo", sortable: false, minWidth: true, isLink: false }) );
      this.columns.push( new DataColumn({ headerText: "PO", value: "poNo", sortable: false, minWidth: true }) );
      this.columns.push( new DataColumn({ headerText: "Inv Date", value: "invoiceDate", sortable: true, isEditableDate: true }) );
      this.columns.push( new DataColumn({ headerText: "ETA", value: "eta", sortable: true, isEditableDate: true }) );
      this.columns.push( new DataColumn({ headerText: "Rcvd On", value: "receivedDate", isDate: true }) );
      this.columns.push( new DataColumn({ headerText: "Rcvd", value: "isInvoiceReceived", isBoolean: true, isDisabled: true, customStyling: 'center' }) );
      this.columns.push( new DataColumn({ headerText: "Inv", isActionColumn: true, customStyling: 'center', actions: [
        new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Primary, event: 'downloadInvoice', icon: 'fa fa-download', showOnlyIf: 'data["invoicePath"] != ""' }),
        new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Warning, event: 'uploadInvoice', icon: 'fa fa-upload', showOnlyIf: 'data["invoicePath"] == ""' })
      ] }) );
      this.columns.push( new DataColumn({ headerText: "Pack", isActionColumn: true, customStyling: 'center', actions: [
        new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Primary, event: 'downloadPackingSlip', icon: 'fa fa-download', showOnlyIf: 'data["packingSlipPath"] != ""' }),
        new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Warning, event: 'uploadPackingSlip', icon: 'fa fa-upload', showOnlyIf: 'data["packingSlipPath"] == ""' })
      ] }) );
      this.columns.push( new DataColumn({ headerText: "10+2", isActionColumn: true, customStyling: 'center', actions: [
        new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Primary, event: 'downloadTenPlus', icon: 'fa fa-download', showOnlyIf: 'data["tenPlusPath"] != ""' }),
        new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Warning, event: 'uploadTenPlus', icon: 'fa fa-upload', showOnlyIf: 'data["tenPlusPath"] == ""' })
      ] }) );
      this.columns.push( new DataColumn({ headerText: "BL", isActionColumn: true, customStyling: 'center', actions: [
        new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Primary, event: 'downloadBl', icon: 'fa fa-download', showOnlyIf: 'data["blPath"] != ""' }),
        new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Warning, event: 'uploadBl', icon: 'fa fa-upload', showOnlyIf: 'data["blPath"] == ""' })
      ] }) );
      this.columns.push( new DataColumn({ headerText: "TC", isActionColumn: true, customStyling: 'center', actions: [
        new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Primary, event: 'downloadTc', icon: 'fa fa-download', showOnlyIf: 'data["tcPath"] != ""' }),
        new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Warning, event: 'uploadTc', icon: 'fa fa-upload', showOnlyIf: 'data["tcPath"] == ""' })
      ] }) );
      this.columns.push( new DataColumn({ headerText: "Action", isActionColumn: true, customStyling: 'center', actions: [
        new DataColumnAction({ actionText: 'Inv', actionStyle: ClassConstants.Primary, event: 'printInvoiceBarcode', icon: 'fa fa-barcode' }),
        new DataColumnAction({ actionText: 'Box', actionStyle: ClassConstants.Primary, event: 'printBoxBarcode', icon: 'fa fa-barcode' }),
        new DataColumnAction({ actionText: 'Receive', actionStyle: ClassConstants.Primary, event: 'receiveInvoice', icon: '', showOnlyIf: 'data["isInvoiceReceived"] == false' }),
        new DataColumnAction({ actionText: 'Unreceive', actionStyle: ClassConstants.Primary, event: 'unReceiveInvoice', icon: '', showOnlyIf: 'data["isInvoiceReceived"] == true' }),
        new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Primary, event: 'updateInvoice', icon: 'fa fa-save' }),
        new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Danger, event: 'deleteInvoice', icon: 'fa fa-trash' })
      ] }) );
    } else {
      this.columns.push( new DataColumn({ headerText: "Supplier", value: "supplierName", sortable: false, minWidth: true }) );
      this.columns.push( new DataColumn({ headerText: "Invoice", value: "invoiceNo", sortable: false, minWidth: true, isLink: false }) );
      this.columns.push( new DataColumn({ headerText: "PO", value: "poNo", sortable: false, minWidth: true }) );
      this.columns.push( new DataColumn({ headerText: "Part Code", value: "partCode", sortable: false, minWidth: true }) );
      this.columns.push( new DataColumn({ headerText: "Qty", value: "quantity", sortable: false, minWidth: true, customStyling: 'right' }) );
      this.columns.push( new DataColumn({ headerText: "Price", value: "rate", sortable: false, minWidth: true, customStyling: 'right' }) );
      this.columns.push( new DataColumn({ headerText: "Total", value: "amount", sortable: false, minWidth: true, customStyling: 'right' }) );
      this.columns.push( new DataColumn({ headerText: "Adj Qty", value: "amount", sortable: false, minWidth: true, customStyling: 'right' }) );
      this.columns.push( new DataColumn({ headerText: "PO's", value: "purchaseOrderNumbers", sortable: false, minWidth: true, customStyling: 'right' }) );
      this.columns.push( new DataColumn({ headerText: "PO Qty's", value: "purchaseOrderQty", sortable: false, minWidth: true, customStyling: 'right' }) );
      this.columns.push( new DataColumn({ headerText: "Excess Qty", value: "excessQty", sortable: false, minWidth: true, customStyling: 'right' }) );
    }
  }

  loadAllSuppliers() {
    this.loaderService.show();
    this.supplierService.getAllSuppliers(this.currentlyLoggedInCompany)
        .pipe(
          map(suppliers => {
            this.suppliers = suppliers;
            this.invoiceForm.get('supplierList').setValue(-1);
            return suppliers;
          }),
          mergeMap(invoices => this.invoiceService.getAllInvoices(this.currentlyLoggedInCompany))
        )
        .subscribe(
          (invoices) => {
            var supplierId = this.invoiceForm.get('supplierList').value;
            this.invoices = supplierId > 0 ? invoices.filter(s => s.supplierId == supplierId): invoices;
            this.filteredInvoices = this.invoices;
          },
          (error) => console.log(error),
          () => { this.loaderService.hide(); }
        );
  }

  supplierSelected() {
    var supplierId = this.invoiceForm.get('supplierList').value;
    this.filteredInvoices = supplierId > 0 ? this.invoices.filter(s => s.supplierId == supplierId): this.invoices;
  }

  filterOptionSelected(event) {
    this.initializeGridColumns();
    this.supplierSelected();

    var showNotReceivedOrdrers = this.invoiceForm.get('showNotReceivedOrders').value;
    this.filteredInvoices = showNotReceivedOrdrers ? this.filteredInvoices.filter(i => i.isInvoiceReceived === false): this.filteredInvoices;

    if (this.invoiceForm.get('showDetails').value) {
      var invoicesForDetails = showNotReceivedOrdrers ? this.filteredInvoices.filter(i => i.isInvoiceReceived === false): this.filteredInvoices;
      this.filteredInvoices = [];

      invoicesForDetails.forEach(invoice => {
        invoice.supplierInvoiceDetails.forEach(detail => {
          var viewModel = new InvoiceListDetailsViewModel();
          viewModel.invoiceNo = invoice.invoiceNo;
          viewModel.poNo = invoice.poNo;
          viewModel.supplierName = invoice.supplierName;
          viewModel.partCode = detail.partDetail.code;
          viewModel.quantity = detail.qty;
          viewModel.rate = detail.price;
          viewModel.amount = detail.total;
          viewModel.adjustedQty = detail.adjustedQty;
          viewModel.excessQty = detail.excessQty;
          detail.supplierInvoicePoDetails.forEach(item => {
            viewModel.purchaseOrderNumbers += `${item.poNo}, `;
            viewModel.purchaseOrderQty += `${item.qty}, `;
          });
          viewModel.purchaseOrderNumbers = viewModel.purchaseOrderNumbers.substring(0, viewModel.purchaseOrderNumbers.length - 2);
          viewModel.purchaseOrderQty = viewModel.purchaseOrderQty.substring(0, viewModel.purchaseOrderQty.length - 2);
          this.filteredInvoices.push(viewModel);
        });
      });
    }
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
      case 'uploadInvoice':
        this.fileAttributes = {'type': 'Invoice', 'file': null, 'name': data.id};
        this.openFileSelector();
        break;
      case 'uploadPackingSlip':
        this.fileAttributes = {'type': 'PackingSlip', 'file': null, 'name': data.id};
        this.openFileSelector();
        break;
      case 'uploadTenPlus':
        this.fileAttributes = {'type': 'TenPlus', 'file': null, 'name': data.id};
        this.openFileSelector();
        break;
      case 'uploadBl':
        this.fileAttributes = {'type': 'BL', 'file': null, 'name': data.id};
        this.openFileSelector();
        break;
      case 'uploadTc':
          this.fileAttributes = {'type': 'TC', 'file': null, 'name': data.id};
          this.openFileSelector();
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
        this.loaderService.show();
        this.invoiceService.receivedInvoice(data.supplierId, data.id)
            .subscribe(() => {
              this.toastr.successToastr('Invoice received successfully!!');
              data.isInvoiceReceived = true;
            }, 
              (error) => { this.toastr.errorToastr(error.error); this.loaderService.hide(); },
              () => this.loaderService.hide()
            );
        break;
      case 'unReceiveInvoice':
        this.loaderService.show();
        this.invoiceService.unReceivedInvoice(data.supplierId, data.id)
            .subscribe(() => {
              this.toastr.successToastr('Invoice unreceived successfully!!');
              data.isInvoiceReceived = false;
            }, 
              (error) => { this.toastr.errorToastr(error.error); this.loaderService.hide(); },
              () => this.loaderService.hide()
            );
        break;
      case 'updateInvoice':
        this.loaderService.show();
        this.invoiceService.updateInvoice(data)
              .subscribe(() => this.toastr.successToastr('Updated successfully!!'),
                        (error) => this.toastr.errorToastr('Error while updating the invoice'),
                        () => this.loaderService.hide());
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

  openFileSelector() {
    let element: HTMLElement = document.querySelector('input[type="file"]') as HTMLElement;
    element.click();
  }

  uploadFile(files: FileList) {
    this.loaderService.show();
    this.fileAttributes.file = files[0];
    this.fileUploadService.uploadFile(this.fileAttributes, this.fileAttributes.name);
    this.loaderService.hide();
    this.toastr.successToastr('Document uploaded successfully. Please refresh to see the latest update.');
  }
}

export class InvoiceListDetailsViewModel {
  supplierName: string;
  invoiceNo: string;
  poNo: string;
  partCode: string;
  quantity: number;
  rate: number;
  amount: number;
  adjustedQty: number;
  purchaseOrderNumbers: string = '';
  purchaseOrderQty: string = '';
  excessQty: number;
}