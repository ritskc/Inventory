import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ClassConstants } from '../../../common/constants';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { CompanyService } from '../../../company/company.service';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { PartsViewModel } from '../../../models/part.model';
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
    this.columns.push( new DataColumn({ headerText: "Description", value: "Description", columnName: 'PartDescription', sortable: true, customStyling: 'column-width-200' }) );
    this.columns.push( new DataColumn({ headerText: "Opening", value: "monthlyOpeningQty", columnName: 'OpeningQty', customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Shipped", value: "shippedQty", columnName: 'Shipped', customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Invoiced", value: "invoiceQty", columnName: 'Invoiced', customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Returned", value: "monthlyReturnQty", columnName: 'ReturnQty', customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Excess", value: "monthlyExcessQty", columnName: 'ExcessQty', customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Rejected", value: "monthlyRejectQty", columnName: 'RejectQty', customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Transit", value: "IntransitQty", columnName: 'InTransitQty', customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Closing", value: "monthlyClosingQty", columnName: 'ClosingQty', customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Action", columnName: 'Action', value: "Action", isActionColumn: true, customStyling: 'center column-width-200', actions: [
      new DataColumnAction({ actionText: 'Create', actionStyle: ClassConstants.Primary, icon: 'fa fa-plus', event: 'createAssembly' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, icon: 'fa fa-trash', event: 'deleteAssembly' })
    ] }) );
  }

  loadAllAssemblies() {
    this.parts = [];
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
      case 'deleteAssembly':
        var qtyToDelete = prompt('Enter the number of assembly quantities to delete');
        if (confirm(`Are you sure to delete ${ qtyToDelete } assembly?`)) {
          var body = {
            partId: data.id,
            qty: qtyToDelete,
            warehouseId: data.part.warehouseId
          };
          this.partService.deleteAnAssembly(data.id, body)
            .subscribe(
              () => this.loadAllAssemblies(),
              (error) => console.log(error.error),
              () => {}
            );
        }
        break;
    }
  }
}