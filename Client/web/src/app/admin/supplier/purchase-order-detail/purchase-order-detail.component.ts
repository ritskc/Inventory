import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { SupplierService } from '../supplier.service';
import { CompanyService } from '../../../company/company.service';
import { ActivatedRoute } from '@angular/router';
import { PurchaseOrder, PurchaseOrderDetail } from '../../../models/purchase-order';
import { DataColumn } from '../../../models/dataColumn.model';

@Component({
  selector: 'app-purchase-order-detail',
  templateUrl: './purchase-order-detail.component.html',
  styleUrls: ['./purchase-order-detail.component.scss']
})
export class PurchaseOrderDetailComponent implements OnInit {

  private currentlyLoggedInCompanyId: number;
  private supplierPurchaseOrderDetailsForm: FormGroup;
  private gridColumns: DataColumn[] = [];
  private purchaseOrder: PurchaseOrder;
  private purchaseOrderList: PurchaseOrder[] = [];
  private purchaseOrderDetails: PurchaseOrderDetail[] = [];
  private supplierId: number = 0;
  private posId: number = 0;

  constructor(private service: SupplierService, private companyService: CompanyService, private formBuilder: FormBuilder,
              private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();

    this.supplierPurchaseOrderDetailsForm = this.formBuilder.group({
      posList: FormControl
    });

    this.loadAllPurchOrdersForSupplier();
    this.loadPurchaseOrderDetails();
  }

  initializeGridColumns() {
    this.gridColumns.push( new DataColumn({ headerText: "Part Code", value: "part", nested: "code", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Name", value: "part", nested: "description", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Quantity", value: "qty", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Ack Quantity", value: "ackQty", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Unit Price", value: "unitPrice", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Total", value: "unitPrice", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Due Date", value: "dueDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Reference", value: "referenceNo", sortable: true }) );
  }

  loadAllPurchOrdersForSupplier() {
    this.supplierId = this.activatedRoute.snapshot.params.supplierId;
    this.posId = this.activatedRoute.snapshot.params.posId;
    this.service.getPurchaseOrders(this.currentlyLoggedInCompanyId)
        .subscribe((posList) => {
          this.purchaseOrderList = posList;
          this.supplierPurchaseOrderDetailsForm.get('posList').setValue(this.posId);
        },
        (error) => console.log(error));
  }

  loadPurchaseOrderDetails() {
    this.service.getPurchaseOrder(this.currentlyLoggedInCompanyId, this.posId)
        .subscribe((purchaseOrder) => this.purchaseOrder = purchaseOrder ),
                  (error) => console.log(error);
  }
}