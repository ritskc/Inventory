import { Component, OnInit } from '@angular/core';
import { SupplierService } from '../supplier.service';
import { CompanyService } from '../../../company/company.service';
import { DataColumn } from '../../../models/dataColumn.model';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Supplier } from '../../../models/supplier.model';
import { UserAction } from '../../../models/enum/userAction';
import { FormControl, FormGroup, FormBuilder } from '@angular/forms';
import { PurchaseOrder } from '../../../models/purchase-order';

@Component({
  selector: 'app-purchase-order-list',
  templateUrl: './purchase-order-list.component.html',
  styleUrls: ['./purchase-order-list.component.scss']
})
export class PurchaseOrderListComponent implements OnInit {

  private purchaseOrders: PurchaseOrder[] = [];
  private gridColumns: DataColumn[] = [];
  private currentlyLoggedInCompanyid: number = 0;
  private supplierId: number = 0;
  private suppliers: Supplier[];
  private supplierForm: FormGroup;

  constructor(private service: SupplierService, private companyService: CompanyService, private formBuilder: FormBuilder,
              private loaderService: httpLoaderService, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyid = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeSupplierForms();
    this.loadAllSuppliers();
    this.initializeGridColumns();
    this.extractSupplierId();
    this.loadAllPurchaseOrders();
  }

  initializeSupplierForms() {
    this.supplierForm = this.formBuilder.group({
      supplierList: FormControl
    });
    if (UserAction[this.activatedRoute.snapshot.params.action] != UserAction[UserAction.ListAll])
      this.supplierForm.get('supplierList').disable();
  }

  initializeGridColumns() {
    this.gridColumns.push( new DataColumn({ headerText: "Number", value: "poNo", isLink: true, sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Email", value: "emailIds", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Date", value: "poDate", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Due Date", value: "closingDate", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Closed", value: "isClosed", sortable: true }) );
  }

  extractSupplierId() {
    this.supplierId = this.activatedRoute.snapshot.params.id;
    this.supplierForm.get('supplierList').setValue(this.supplierId);
  }

  loadAllSuppliers(){
    this.loaderService.show();
    this.service.getAllSuppliers(this.currentlyLoggedInCompanyid)
        .subscribe((suppliers) => {
          this.suppliers = suppliers;
          this.loaderService.hide();
        }, (error) => {
          console.log(error);
          this.loaderService.hide();
        });
  }

  loadAllPurchaseOrders() {
    this.loaderService.show();
    this.service.getPurchaseOrders(this.supplierId)
        .subscribe((purchaseOrders) => {
          this.purchaseOrders = purchaseOrders;
          this.loaderService.hide();
        }, (error) => {
          this.loaderService.hide();
        });
  }

  rowSelected(event) {
    console.log(event);
    this.redirectToPurchaseOrderDetails(event);
  }

  redirectToPurchaseOrderDetails(row: any){
    this.router.navigateByUrl(`/suppliers/pos/1/2`);
  }
}
