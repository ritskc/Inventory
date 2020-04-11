import { Component, OnInit } from '@angular/core';
import { Supplier } from '../../../models/supplier.model';
import { Invoice } from '../../../models/invoice.model';
import { CompanyService } from '../../../company/company.service';
import { SupplierService } from '../../supplier/supplier.service';
import { InvoiceService } from '../invoice.service';
import { DataColumn } from '../../../models/dataColumn.model';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-receive',
  templateUrl: './receive.component.html',
  styleUrls: ['./receive.component.scss']
})
export class ReceiveComponent implements OnInit {

  private currentlyLoggedInCompanyId: number = 0;
  private supplierInvoice: Invoice;
  private suppliers: Supplier[] = [];
  private supplierInvoices: Invoice[] = [];
  private columns: DataColumn[] = [];
  private selectedSupplier = -1; 
  private selectedInvoice = -1;

  constructor(private companyService: CompanyService, private supplierService: SupplierService, private service: InvoiceService,
              private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.supplierInvoice = new Invoice();
    this.loadSuppliers();
    this.prepareGridColumns();
  }

  prepareGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Part Code", value: "partCode", isLink: true, sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Quantity", value: "qty", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Price", value: "price", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Total", value: "total", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Box No", value: "boxNo", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Multiple PO", value: "boxNo" }) );
    this.columns.push( new DataColumn({ headerText: "Multiple Qty", value: "boxNo" }) );
    this.columns.push( new DataColumn({ headerText: "Excess", value: "boxNo" }) );
  }

  loadSuppliers() {
    this.supplierService.getAllSuppliers(this.currentlyLoggedInCompanyId)
        .subscribe(
          (suppliers) => { 
            this.suppliers = suppliers;
            if (this.activatedRoute.snapshot.params.id) {
              this.selectedSupplier = this.activatedRoute.snapshot.params.id;
              this.supplierSelected(null);
            }            
          },
          (error) => { console.log(error); },
          () => { }
        );
  }

  supplierSelected(event) {
    var selectedSupplier = event ? event.target.value: this.activatedRoute.snapshot.params.id;
    this.supplierInvoices = [];
    this.supplierInvoice = new Invoice();
    
    this.service.getAllInvoices(this.currentlyLoggedInCompanyId)
        .subscribe(
          (invoices) => {
            var invalidInvoice = new Invoice();
            invalidInvoice.id = -1;

            this.supplierInvoices.push(invalidInvoice);
            invoices.forEach((invoice) => {
              if (invoice.supplierId == selectedSupplier)
                this.supplierInvoices.push(invoice);
            })
          },
          (error) => { console.log(error); },
          () => { }
        );
  }

  invoiceSelected(event) {
    this.supplierInvoice = this.supplierInvoices.find(i => i.id == event.target.value);
  }

  invoiceReceived() {
    this.service.receivedInvoice(this.supplierInvoice.supplierId, this.supplierInvoice.id)
        .subscribe(
          (result) => { alert('Invoice received successfully!'); },
          (error) => { console.log(error) }
        );
  }
}