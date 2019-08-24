import { Component, OnInit } from '@angular/core';
import { Part, PartsViewModel } from '../../models/part.model';
import { DataColumn } from '../../models/dataColumn.model';
import { PartsService } from '../../admin/parts/parts.service';
import { CompanyService } from '../company.service';

@Component({
  selector: 'app-inventory-parts-list',
  templateUrl: './inventory-parts-list.component.html',
  styleUrls: ['./inventory-parts-list.component.scss']
})
export class InventoryPartsListComponent implements OnInit {

  parts: PartsViewModel[] = [];
  columns: DataColumn[] = [];
  currentlyLoggedInCompanyId: number = 0;

  constructor(private service: PartsService, private companyService: CompanyService) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.getAllPartsForCompany();
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Code", value: "Code", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "Description", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Min Qty", value: "MinQty", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Max Qty (Lbs)", value: "MaxQty", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Safe Qty", value: "SafeQty", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Qty In Hand", value: "QuantityInHand", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "In Transit", value: "IntransitQty", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Total", value: "Total", sortable: false }) );
  }

  getAllPartsForCompany() {
    this.service.getAllParts(this.currentlyLoggedInCompanyId)
        .subscribe((parts) => {
          parts.forEach((part) => {
            this.parts.push(new PartsViewModel(part));
            console.log(this.parts);
          })
        },
        (error) => {
          console.log(error);
        }
      );
  }

}
