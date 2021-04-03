import { Component, OnInit } from '@angular/core';
import { PartsService } from '../../admin/parts/parts.service';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { DataColumn } from '../../models/dataColumn.model';
import { CompanyService } from '../company.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  minQtyGridColumn: DataColumn[] = [];
  zeroQtyPartsGridColumn: DataColumn[] = [];
  zeroInTransitQtyPartsGridColumn: DataColumn[] = [];
  minQtyParts: any[] = [];
  zeroQtyParts: any[] = [];
  zeroInTransitParts: any[] = [];

  currentlyLoggedInCompany: number = 0;

  constructor(private httpLoaderService: httpLoaderService, private companyService: CompanyService, private partService: PartsService) { }

  ngOnInit() {
    this.minQtyGridColumn.push(new DataColumn({ headerText: "Part", value: "description", sortable: false }));
    this.minQtyGridColumn.push(new DataColumn({ headerText: "Supplier", value: "supplierCode", sortable: false }));
    this.minQtyGridColumn.push(new DataColumn({ headerText: "Customer", value: "customerName", sortable: false }));
    this.minQtyGridColumn.push(new DataColumn({ headerText: "In Hand", value: "qtyInHand", sortable: false }));
    this.minQtyGridColumn.push(new DataColumn({ headerText: "Min", value: "minQty", sortable: false }));

    this.zeroQtyPartsGridColumn.push(new DataColumn({ headerText: "Part", value: "description", sortable: false }));
    this.zeroQtyPartsGridColumn.push(new DataColumn({ headerText: "Supplier", value: "supplierCode", sortable: false }));
    this.zeroQtyPartsGridColumn.push(new DataColumn({ headerText: "Customer", value: "customerName", sortable: false }));

    this.zeroInTransitQtyPartsGridColumn.push(new DataColumn({ headerText: "Part", value: "description", sortable: false }));
    this.zeroInTransitQtyPartsGridColumn.push(new DataColumn({ headerText: "Supplier", value: "supplierCode", sortable: false }));
    this.zeroInTransitQtyPartsGridColumn.push(new DataColumn({ headerText: "Customer", value: "customerName", sortable: false }));

    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.getPartsForDashboard();
  }

  getPartsForDashboard() {
    this.httpLoaderService.show();
    this.partService.getPartsForDashboard(this.currentlyLoggedInCompany)
        .subscribe(
          (parts: any[]) => { 
            this.minQtyParts = parts.filter(p => p.qtyInHand < p.minQty);
            this.zeroQtyParts = parts.filter(p => p.qtyInHand == 0);
            this.zeroInTransitParts = parts.filter(p => p.intransitQty == 0);
           },
          (error) => console.log(error),
          () => this.httpLoaderService.hide()
        );
  }
}