import { Component, OnInit } from '@angular/core';
import readXlsxFile from 'read-excel-file';
import { CompanyService } from '../../../company/company.service';
import { DataColumn } from '../../../models/dataColumn.model';
import { InvoiceDetail, UploadInvoice, UploadInvoiceDetail } from '../../../models/invoice.model';
import { Utils } from '../../../common/utils/utils';
import { InvoiceService } from '../invoice.service';

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

  constructor(private companyService: CompanyService, private invoiceService: InvoiceService) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeColumns();
  }

  initializeColumns() {
    this.columns.push( new DataColumn({ headerText: "Part Code", value: "PartCode" }) );
    this.columns.push( new DataColumn({ headerText: "Quantity", value: "Qty" }) );
    this.columns.push( new DataColumn({ headerText: "Rate", value: "Price" }) );
    this.columns.push( new DataColumn({ headerText: "Amount", value: "Total" }) );
  }

  uploadFile(files: FileList) {
    readXlsxFile(files[0]).then((rows) => {
      console.log(rows);
      this.extractDataFromFile(rows);
    });
  }

  extractDataFromFile(rows: any) {
    this.invoiceNo = rows[1][0];
    this.invoiceDate = Utils.DateToString(new Date(rows[1][1]));
    this.supplierName = rows[1][2];
    this.poNo = rows[1][3];
    this.company = rows[1][4];
    
    for (let index = 2; index < rows.length; index++) {
      var invoiceDetail = new UploadInvoiceDetail();
      invoiceDetail.PartCode = rows[index][5];
      invoiceDetail.Qty = rows[index][6];
      invoiceDetail.Price = rows[index][7];
      invoiceDetail.Total = invoiceDetail.Qty * invoiceDetail.Price;
      this.invoice.supplierInvoiceDetails.push(invoiceDetail);
    }
  }

  uploadInvoice() {
    this.invoice.InvoiceNo = this.invoiceNo;
    this.invoice.InvoiceDate = this.invoiceDate.toString();
    this.invoice.SupplierName = this.supplierName;
    this.invoice.PoNo = this.poNo;
    this.invoice.CompanyName = this.company;
    this.invoice.ETA = this.eta ? this.eta: new Date().toLocaleString();
    this.invoice.UploadedDate = new Date().toLocaleString();
    this.invoiceService.uploadInvoice(this.invoice)
        .subscribe((invoiceNumber) => {
          alert(invoiceNumber);
        });
  }
}
