import { Component, OnInit } from '@angular/core';
import readXlsxFile from 'read-excel-file';
import { CompanyService } from '../../../company/company.service';
import { DataColumn } from '../../../models/dataColumn.model';
import { InvoiceDetail, UploadInvoice, UploadInvoiceDetail, UploadMode, Invoice } from '../../../models/invoice.model';
import { Utils } from '../../../common/utils/utils';
import { InvoiceService } from '../invoice.service';
import { FileUploadService } from '../../../common/services/file-upload.service';
import { HttpRequest, HttpClient, HttpEventType, HttpResponse } from '@angular/common/http';
import { Subject } from 'rxjs';
import { ToastrManager } from 'ng6-toastr-notifications';
import * as DateHelper from '../../../common/helpers/dateHelper';

@Component({
  selector: 'app-upload-invoice',
  templateUrl: './upload-invoice.component.html',
  styleUrls: ['./upload-invoice.component.scss']
})
export class UploadInvoiceComponent implements OnInit {

  private invoiceToValidate: UploadInvoice = new UploadInvoice();
  private invoice: Invoice = new Invoice();
  private currentlyLoggedInCompany: number = 0;
  private columns: DataColumn[] = [];
  private invoiceNo: string = '';
  private poNo: string = '';
  private invoiceDate: string = '';
  private supplierName: string = '';
  private company: string = '';
  private eta: string = '';
  private invoiceTotal: number = 0;
  private documents = [];

  constructor(private companyService: CompanyService, private invoiceService: InvoiceService, private fileService: FileUploadService,
              private http: HttpClient, private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
  }

  initializeColumns(mode: UploadMode = UploadMode.Confirm) {
    this.columns = [];
    this.columns.push( new DataColumn({ headerText: "Valid", value: "isValid", isBoolean: true, customStyling: 'center' }) );
    this.columns.push( new DataColumn({ headerText: "Part Code", value: "partCode" }) );
    this.columns.push( new DataColumn({ headerText: "Quantity", value: "qty", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Rate", value: "price", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Amount", value: "total", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Adjusted Qty", value: "adjustedPOQty", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Excess Qty", value: "excessQty", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Box Number", value: "boxNo" }) );
  }

  uploadFile(files: FileList) {
    readXlsxFile(files[0]).then((rows) => {
      this.extractDataFromFile(rows);
      this.validateInvoice(UploadMode.Validate);
    });
  }

  uploadAttachment(files: FileList, folderName: string) {
    this.documents.push({'type': folderName, 'file': files[0]});
    return;


    // var formData = new FormData();
    // formData.append('file', files[0], files[0].name);
    // const req = new HttpRequest('POST', 'http://po.harisons.com/api/File/BL/10', formData, {
    //   reportProgress: true
    // });
    
    // const progress = new Subject<number>();

    // this.http.request(req).subscribe(event => {
    //   if (event.type === HttpEventType.UploadProgress) {

    //     // calculate the progress percentage
    //     const percentDone = Math.round(100 * event.loaded / event.total);

    //     // pass the percentage into the progress-stream
    //     progress.next(percentDone);
    //   } else if (event instanceof HttpResponse) {

    //     // Close the progress-stream if we get an answer form the API
    //     // The upload is complete
    //     progress.complete();
    //   }
    // });
    //this.fileService.postFile(files.item(0));
  }

  extractDataFromFile(rows: any) {
    this.invoiceNo = rows[1][0];
    this.invoiceDate = Utils.DateToString(new Date(rows[1][1]));
    this.supplierName = rows[1][2];
    this.poNo = rows[1][3];
    this.invoiceTotal = rows[1][4];
    this.company = rows[1][5];
    
    this.invoiceToValidate.supplierInvoiceDetails = [];
    for (let index = 1; index < rows.length; index++) {
      var invoiceDetail = new UploadInvoiceDetail();
      invoiceDetail.PartCode = rows[index][6];
      invoiceDetail.Qty = rows[index][7];
      invoiceDetail.Price = rows[index][8];
      invoiceDetail.Total = invoiceDetail.Qty * invoiceDetail.Price;
      invoiceDetail.BoxNo = rows[index][11];
      this.invoiceToValidate.supplierInvoiceDetails.push(invoiceDetail);
    }
  }

  validateInvoice(mode: UploadMode = UploadMode.Confirm) {
    this.invoiceToValidate.InvoiceNo = this.invoiceNo;
    this.invoiceToValidate.InvoiceDate = this.invoiceDate.toString();
    this.invoiceToValidate.SupplierName = this.supplierName;
    this.invoiceToValidate.PoNo = this.poNo;
    this.invoiceToValidate.InvoiceTotal = this.invoiceTotal;
    this.invoiceToValidate.CompanyName = this.company;
    this.invoiceToValidate.ETA = this.eta ? this.eta: new Date().toLocaleString();
    this.invoiceToValidate.UploadedDate = new Date().toLocaleString();
    this.invoiceService.validateInvoice(this.invoiceToValidate)
        .subscribe((response) => {
          this.invoice = response as Invoice;
          this.initializeColumns();
          this.checkForInvalidParts();
          console.log(this.invoice);
        });
  }

  uploadInvoice() {
    if (this.invoice.supplierInvoiceDetails.filter(d => !d.isValid).length > 0) {
      alert('This invoice cannot be uploaded due to one ore more invalid parts. Please check the result above');
      return;
    }

    this.invoiceService.uploadInvoice(this.invoice)
        .subscribe(response => {
          var invoiceResponse = response as Invoice;
          this.uploadDocuments(invoiceResponse.id.toString());
          this.toastr.successToastr('Invoice uploaded successfully!!');
        })
  }

  uploadDocuments(invoiceNumber: string) {
    this.documents.forEach((item) => {
      this.fileService.uploadFile(item, invoiceNumber);
      this.toastr.successToastr('File(s) uploaded successfully!!');
    });
  }

  private checkForInvalidParts() {
    this.invoice.supplierInvoiceDetails.forEach(item => {
      item.isValid = item.partId > 0;
    });
  }
}