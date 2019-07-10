import { Component, OnInit } from '@angular/core';
import readXlsxFile from 'read-excel-file';
import { CompanyService } from '../../../company/company.service';
import { DataColumn } from '../../../models/dataColumn.model';
import { PurchaseOrderDetail } from '../../../models/purchase-order';

@Component({
  selector: 'app-upload-invoice',
  templateUrl: './upload-invoice.component.html',
  styleUrls: ['./upload-invoice.component.scss']
})
export class UploadInvoiceComponent implements OnInit {

  private currentlyLoggedInCompany: number = 0;
  private columns: DataColumn[] = [];
  private parts: PurchaseOrderDetail[] = [];
  private invoiceNo: string = '';
  private poNo: string = '';
  private invoiceDate: string = '';
  private supplierName: string = '';
  private company: string = '';

  constructor(private companyService: CompanyService) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeColumns();
  }

  initializeColumns() {
    this.columns.push( new DataColumn({ headerText: "Part Code", value: "partCode" }) );
    this.columns.push( new DataColumn({ headerText: "Quantity", value: "qty" }) );
    this.columns.push( new DataColumn({ headerText: "Rate", value: "unitPrice" }) );
    this.columns.push( new DataColumn({ headerText: "Amount", value: "total" }) );
  }

  uploadFile(files: FileList) {
    readXlsxFile(files[0]).then((rows) => {
      console.log(rows);
      this.extractDataFromFile(rows);
    });
  }

  extractDataFromFile(rows: any) {
    this.invoiceNo = rows[1][0];
    this.invoiceDate = rows[1][1];
    this.supplierName = rows[1][2];
    this.poNo = rows[1][3];
    this.company = rows[1][4];
    
    for (let index = 2; index < rows.length; index++) {
      var poDetails = new PurchaseOrderDetail();
      poDetails.partCode = rows[index][5];
      poDetails.qty = rows[index][6];
      poDetails.unitPrice = rows[index][7];
      poDetails.total = poDetails.qty * poDetails.unitPrice;
      this.parts.push(poDetails);
    }
  }
}
