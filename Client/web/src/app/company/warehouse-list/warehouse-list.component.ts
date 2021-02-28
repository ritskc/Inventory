import { Component, OnInit } from '@angular/core';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { DataColumn } from '../../models/dataColumn.model';
import { CompanyService } from '../company.service';

@Component({
  selector: 'app-warehouse-list',
  templateUrl: './warehouse-list.component.html',
  styleUrls: ['./warehouse-list.component.scss']
})
export class WarehouseListComponent implements OnInit {

  private data: any[] = [];
  private columns: DataColumn[] = [];

  constructor(private loaderAnimation: httpLoaderService, private companyService: CompanyService) { }

  ngOnInit() {
    this.prepareColumnForGrid();
    this.getWarehouseReportData();
  }

  prepareColumnForGrid() {
    this.columns.push( new DataColumn({ headerText: "Code", value: "code", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "description", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "From", value: "fromWarehouse", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "To", value: "toWarehouse", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Quantity", value: "qty", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Action Time", value: "actionTime", isDateTime: true, sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Direction", value: "direction", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Transaction Type", value: "transactionType", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Reference", value: "referenceNo", sortable: false }) );
  }

  getWarehouseReportData() {
    this.loaderAnimation.show();
    this.companyService.getWarehouseReport()
        .subscribe(
          (data) => this.data = data,
          (error) => console.log(error.error),
          () => this.loaderAnimation.hide()
        );
  }
}