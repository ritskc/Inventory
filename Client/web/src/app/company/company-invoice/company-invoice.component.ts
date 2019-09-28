import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../company.service';
import { Router } from '@angular/router';
import { DataColumn } from '../../models/dataColumn.model';

@Component({
  selector: 'app-company-invoice',
  templateUrl: './company-invoice.component.html',
  styleUrls: ['./company-invoice.component.scss']
})
export class CompanyInvoiceComponent implements OnInit {

  private currentlyLoggedInCompany: number = 0;
  columns: DataColumn[] = [];

  constructor(private companyService: CompanyService, private router: Router) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Supplier", value: "supplierName", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Invoice", value: "invoiceNo", sortable: false }) );
  }

  addInvoice() {
    
  }
}
