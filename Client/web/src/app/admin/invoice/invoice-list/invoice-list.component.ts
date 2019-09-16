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

@Component({
  selector: 'app-invoice-list',
  templateUrl: './invoice-list.component.html',
  styleUrls: ['./invoice-list.component.scss']
})
export class InvoiceListComponent implements OnInit {

  private invoiceForm: FormGroup;
  private currentlyLoggedInCompany: number = 0;
  suppliers: Supplier[] = [];
  invoices: Invoice[] = [];
  columns: DataColumn[] = [];

  constructor(private companyService: CompanyService, private invoiceService: InvoiceService, private supplierService: SupplierService,
              private formBuilder: FormBuilder, private router: Router
    ) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.loadAllSuppliers();
    this.loadAllSupplierInvoices();
    this.invoiceForm = this.formBuilder.group({
      supplierList: FormControl
    })
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Supplier", value: "supplierName", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Invoice", value: "invoiceNo", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Invoice Date", value: "invoiceDate", sortable: true, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "PO", value: "poNo", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "ETA", value: "eta", sortable: true, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Products", constantText: "View", isLink: true }) );
    this.columns.push( new DataColumn({ headerText: "Invoice", constantText: "View", isLink: true }) );
    this.columns.push( new DataColumn({ headerText: "Packing", constantText: "View", isLink: true }) );
    this.columns.push( new DataColumn({ headerText: "10+2", constantText: "View", isLink: true }) );
    this.columns.push( new DataColumn({ headerText: "BL", constantText: "View", isLink: true }) );
    this.columns.push( new DataColumn({ headerText: "TC", constantText: 'View', isLink: true }));
    this.columns.push( new DataColumn({ headerText: "Received", value: "receivedDate", sortable: true, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Action", isActionColumn: true, actions: [
      new DataColumnAction({ actionText: 'Barcode', actionStyle: ClassConstants.Primary, event: 'printBarcode' })
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
    this.invoiceService.getAllInvoices(this.currentlyLoggedInCompany)
        .subscribe(
          (invoices) => {this.invoices = invoices; console.log(this.invoices); },
          (error) => console.log(error),
          () => console.log('completed')
        );
  }

  supplierSelected() {
    var supplierId = this.invoiceForm.get('supplierList').value;
    console.log(supplierId);
    this.invoiceService.getAllSupplierInvoices(this.currentlyLoggedInCompany, supplierId)
        .subscribe((invoices) => this.invoices = invoices)
  }

  uploadInvoice() {
    var selectedSupplierId = this.invoiceForm.get('supplierList').value;
    if (selectedSupplierId > -1)
      this.router.navigateByUrl(`/invoice/upload/${selectedSupplierId}`);
    else
      alert('Please select a supplier to proceed to upload');
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'printBarcode':
        alert(true);
        break;
    }
  }
}
