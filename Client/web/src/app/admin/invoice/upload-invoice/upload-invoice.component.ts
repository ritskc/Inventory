import { Component, OnInit } from '@angular/core';
import readXlsxFile from 'read-excel-file';
import { CompanyService } from '../../../company/company.service';
import { DataColumn } from '../../../models/dataColumn.model';
import { InvoiceDetail, UploadInvoice, UploadInvoiceDetail } from '../../../models/invoice.model';
import { Utils } from '../../../common/utils/utils';
import { InvoiceService } from '../invoice.service';
import { FileUploadService } from '../../../common/services/file-upload.service';
import { HttpRequest, HttpClient, HttpEventType, HttpResponse } from '@angular/common/http';
import { Subject } from 'rxjs';
import { ToastrManager } from 'ng6-toastr-notifications';

@Component({
  selector: 'app-upload-invoice',
  templateUrl: './upload-invoice.component.html',
  styleUrls: ['./upload-invoice.component.scss']
})
export class UploadInvoiceComponent implements OnInit {

  private invoice: UploadInvoice = new UploadInvoice();
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
    this.initializeColumns();
  }

  initializeColumns() {
    this.columns.push( new DataColumn({ headerText: "Part Code", value: "PartCode" }) );
    this.columns.push( new DataColumn({ headerText: "Quantity", value: "Qty" }) );
    this.columns.push( new DataColumn({ headerText: "Rate", value: "Price" }) );
    this.columns.push( new DataColumn({ headerText: "Amount", value: "Total" }) );
    this.columns.push( new DataColumn({ headerText: "Box Number", value: "BoxNumber" }) );
  }

  uploadFile(files: FileList) {
    readXlsxFile(files[0]).then((rows) => {
      console.log(rows);
      this.extractDataFromFile(rows);
    });
  }

  uploadAttachment(files: FileList, folderName: string) {
    this.documents.push({'type': folderName, 'file': files[0]});
    return;


    var formData = new FormData();
    formData.append('file', files[0], files[0].name);
    const req = new HttpRequest('POST', 'http://po.harisons.com/api/File/BL/10', formData, {
      reportProgress: true
    });
    
    const progress = new Subject<number>();

    this.http.request(req).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {

        // calculate the progress percentage
        const percentDone = Math.round(100 * event.loaded / event.total);

        // pass the percentage into the progress-stream
        progress.next(percentDone);
      } else if (event instanceof HttpResponse) {

        // Close the progress-stream if we get an answer form the API
        // The upload is complete
        progress.complete();
      }
    });
    //this.fileService.postFile(files.item(0));
  }

  extractDataFromFile(rows: any) {
    this.invoiceNo = rows[1][0];
    this.invoiceDate = Utils.DateToString(new Date(rows[1][1]));
    this.supplierName = rows[1][2];
    this.poNo = rows[1][3];
    this.invoiceTotal = rows[1][4];
    this.company = rows[1][5];
    
    this.invoice.supplierInvoiceDetails = [];
    for (let index = 1; index < rows.length; index++) {
      var invoiceDetail = new UploadInvoiceDetail();
      invoiceDetail.PartCode = rows[index][6];
      invoiceDetail.Qty = rows[index][7];
      invoiceDetail.Price = rows[index][8];
      invoiceDetail.Total = invoiceDetail.Qty * invoiceDetail.Price;
      invoiceDetail.BoxNumber = rows[index][11];
      this.invoice.supplierInvoiceDetails.push(invoiceDetail);
    }
  }

  uploadInvoice() {
    this.invoice.InvoiceNo = this.invoiceNo;
    this.invoice.InvoiceDate = this.invoiceDate.toString();
    this.invoice.SupplierName = this.supplierName;
    this.invoice.PoNo = this.poNo;
    this.invoice.InvoiceTotal = this.invoiceTotal;
    this.invoice.CompanyName = this.company;
    this.invoice.ETA = this.eta ? this.eta: new Date().toLocaleString();
    this.invoice.UploadedDate = new Date().toLocaleString();
    this.invoiceService.uploadInvoice(this.invoice)
        .subscribe((invoiceNumber) => {
          var invoice = invoiceNumber as string;
          this.uploadDocuments(invoice);
          this.toastr.successToastr('Invoice uploaded successfully!!');
        });
  }

  uploadDocuments(invoiceNumber: string) {
    this.documents.forEach((item) => {
      this.fileService.uploadFile(item, invoiceNumber);
      this.toastr.successToastr('File uploaded successfully!!');
    });
  }
}
