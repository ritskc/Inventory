import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ClassConstants } from '../../../common/constants';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { CompanyService } from '../../../company/company.service';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { Part, PartsViewModel } from '../../../models/part.model';
import { PartsService } from '../parts.service';

@Component({
  selector: 'app-assembly-list',
  templateUrl: './assembly-list.component.html',
  styleUrls: ['./assembly-list.component.scss']
})
export class AssemblyListComponent implements OnInit {

  currentlyLoggedInCompany: number = 0;

  parts: PartsViewModel[] = [];
  columns: DataColumn[] = [];

  constructor(private companyService: CompanyService, private partService: PartsService, 
              private loaderService: httpLoaderService, private router: Router) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.populateGridColumns();
    this.loadAllAssemblies();
  }

  populateGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Code", value: "Code", columnName: 'PartCode', sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "Description", columnName: 'PartDescription', sortable: true, customStyling: 'column-width-150' }) );
    this.columns.push( new DataColumn({ headerText: "Opening Qty", value: "monthlyOpeningQty", columnName: 'OpeningQty', sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Shipped", value: "shippedQty", columnName: 'Shipped' }) );
    this.columns.push( new DataColumn({ headerText: "Invoiced", value: "invoiceQty", columnName: 'Invoiced' }) );
    this.columns.push( new DataColumn({ headerText: "Return Qty", value: "monthlyReturnQty", columnName: 'ReturnQty' }) );
    this.columns.push( new DataColumn({ headerText: "Excess Qty", value: "monthlyExcessQty", columnName: 'ExcessQty' }) );
    this.columns.push( new DataColumn({ headerText: "Reject Qty", value: "monthlyRejectQty", columnName: 'RejectQty' }) );
    this.columns.push( new DataColumn({ headerText: "In Transit", value: "IntransitQty", columnName: 'InTransitQty', sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Closing Qty", value: "monthlyClosingQty", columnName: 'ClosingQty' }) );
    this.columns.push( new DataColumn({ headerText: "Action", columnName: 'Action', value: "Action", isActionColumn: true, customStyling: 'center column-width-150', actions: [
      new DataColumnAction({ actionText: 'Create Assembly', actionStyle: ClassConstants.Primary, event: 'createAssembly' })
    ] }) );
  }

  loadAllAssemblies() {
    this.loaderService.show();
    this.partService.getAllAssembliesInCompany(this.currentlyLoggedInCompany)
        .subscribe(
          (parts) => {
            parts.forEach((part) => {
              this.parts.push(new PartsViewModel(part));
            });
          },
          (error) => console.log(error.error),
          () => this.loaderService.show()
        );
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'createAssembly':
        this.router.navigateByUrl(`parts/assembly/detail/${ data.id }`);
        break;
    }
  }
}