import { Component, OnInit } from '@angular/core';
import { Part } from '../../../models/part.model';
import { PartsService } from '../parts.service';
import { ActivatedRoute, Router } from '@angular/router';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { CompanyService } from '../../../company/company.service';
import { DataColumn } from '../../../models/dataColumn.model';
import { UserAction } from '../../../models/enum/userAction';

@Component({
  selector: 'app-part-list',
  templateUrl: './part-list.component.html',
  styleUrls: ['./part-list.component.scss']
})
export class PartListComponent implements OnInit {

  parts: Part[] = [];
  columns: DataColumn[] = [];
  currentlyLoggedInCompanyId: number = 0;

  constructor(private service: PartsService, private activatedRoute: ActivatedRoute, private router: Router,
              private httpLoader: httpLoaderService, private companyService: CompanyService) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.getAllPartsForCompany();
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Code", value: "code", isLink: true, sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "description", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Weight (Kgs)", value: "weightInKg", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Weight (Lbs)", value: "weightInLb", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Min Qty", value: "minQty", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Max Qty (Lbs)", value: "maxQty", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Active", value: "isActive", sortable: true, isBoolean: true }) );
    this.columns.push( new DataColumn({ headerText: "Sample", value: "isSample", sortable: true, isBoolean: true }) );
  }

  getAllPartsForCompany() {
    this.httpLoader.show();
    this.service.getAllParts(this.currentlyLoggedInCompanyId)
        .subscribe((parts) => {
          this.parts = parts;
          this.httpLoader.hide();
        },
        (error) => {
          console.log(error);
          this.httpLoader.hide();
        }
      );
  }

  rowSelected(row) {
    this.router.navigateByUrl(`/parts/detail/${ UserAction.Edit }/${row.id}`);
  }

  addPart() {
    this.router.navigateByUrl(`/parts/detail/${ UserAction.Add }/#`);
  }
}