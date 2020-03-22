import { Component, OnInit } from '@angular/core';
import { SupplierAccessService } from '../supplier-access.service';
import { ActivatedRoute } from '@angular/router';
import { DataColumn } from '../../models/dataColumn.model';
import { httpLoaderService } from '../../common/services/httpLoader.service';

@Component({
  selector: 'app-direct-supplier-po',
  templateUrl: './direct-supplier-po.component.html',
  styleUrls: ['./direct-supplier-po.component.scss']
})
export class DirectSupplierPoComponent implements OnInit {

  supplerPurchaseOrder: any;
  columns: DataColumn[] = [];
  detail: boolean = false;

  constructor(private supplierAccessService: SupplierAccessService, private activatedRoute: ActivatedRoute, 
              private httpLoaderService: httpLoaderService) { }

  ngOnInit() {
    this.initializeGrid();
    this.getDirectSupplierPurchaseOrder(this.activatedRoute.snapshot.params.id);
  }

  initializeGrid() {
    this.columns = [];
    this.columns.push( new DataColumn({ headerText: "Sr No", value: "srNo" }) );
    this.columns.push( new DataColumn({ headerText: "Part Code", value: "partCode" }) );
    this.columns.push( new DataColumn({ headerText: "Part Code", value: "partDescription" }) );
    this.columns.push( new DataColumn({ headerText: "Quantity", value: "qty" }) );
    this.columns.push( new DataColumn({ headerText: "Ack Quantity", value: "ackQty", isEditable: true }) );
    this.columns.push( new DataColumn({ headerText: "Unit Price", value: "unitPrice" }) );
    this.columns.push( new DataColumn({ headerText: "Total", value: "total" }) );
    this.columns.push( new DataColumn({ headerText: "Due Date", value: "dueDate", isEditableDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Reference", value: "referenceNo" }) );
    this.columns.push( new DataColumn({ headerText: "Notes", value: "note" }) );
  }

  getDirectSupplierPurchaseOrder(id: string) {
    this.httpLoaderService.show();
    this.supplierAccessService.getDirectSupplierPurchaseOrder(id)
        .subscribe(
          result => {
            this.supplerPurchaseOrder = result;
            this.transformData();
          },
          (error) => console.log(error),
          () => { this.httpLoaderService.hide() }
        );
  }

  submitPurchaseOrder() {
    this.httpLoaderService.show();
    this.supplerPurchaseOrder.isAcknowledged = true;
    this.supplierAccessService.submitSupplierPurchaseOrder(this.supplerPurchaseOrder, this.activatedRoute.snapshot.params.id)
        .subscribe(() => {
          alert('Order acknowledged successfully!');
        }, (error) => console.log(error),
        () => this.httpLoaderService.hide());
  }

  showDetails() {
    alert(this.detail);
  }

  private transformData() {
    this.supplerPurchaseOrder.poDetails.forEach(item => {
      item.partCode = item.part.code;
      item.partDescription = item.part.description;
      item.total = item.qty * item.unitPrice;
    });
  }
}