import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrManager } from 'ng6-toastr-notifications';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { CompanyService } from '../../../company/company.service';
import { DataColumn } from '../../../models/dataColumn.model';
import { Part } from '../../../models/part.model';
import { PartsService } from '../parts.service';

@Component({
  selector: 'app-assembly-detail',
  templateUrl: './assembly-detail.component.html',
  styleUrls: ['./assembly-detail.component.scss']
})
export class AssemblyDetailComponent implements OnInit {

  currentlyLoggedInCompany: number = 0;
  currentlyLoggedInWarehouse: number = 0;
  assembly: Part;
  quantity: number = 0;
  warehouseId: number = 0;

  columns: DataColumn[] = [];

  constructor(private partService: PartsService, private companyService: CompanyService, private toastr: ToastrManager,
              private router: Router, private loadingAnimationService: httpLoaderService, private activateRoute: ActivatedRoute) { }

  ngOnInit() {    
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();

    this.columns.push( new DataColumn({ headerText: "Code", value: "subCode", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "subDescription", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Current Inventory", value: "currentInventory", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Required Quantity", value: "requiredQty", sortable: false, customStyling: 'right' }) );

    this.loadingAnimationService.show();
    this.partService.getAnAssembly(this.currentlyLoggedInCompany, this.activateRoute.snapshot.params.id)
        .subscribe(
          (assembly) => this.assembly = assembly,
          (error) => console.log(error.error),
          () => this.loadingAnimationService.hide()
        );
  }

  save() {
    if (this.quantity < 1) {
      this.toastr.warningToastr('Please enter a valid quantity');
      return;
    }

    var validToSave = true;
    this.assembly.partPartAssignments.forEach(partAssignment => {
      if (partAssignment.currentInventory - (partAssignment.requiredQty * this.quantity) < 0)
        validToSave = false;
    });

    if (!validToSave) {
      this.toastr.warningToastr(`Cannot create ${ this.quantity } assemblies since the inventory of few parts will go negative`);
      return false;
    }

    this.loadingAnimationService.show();

    let assembly = {
      partId: this.assembly.id,
      qty: this.quantity,
      warehouseId: this.assembly.warehouseId
    };

    this.partService.createAssembly(assembly)
        .subscribe(
          (result) => console.log(result),
          (error) => console.log(error.error),
          () => {
            this.loadingAnimationService.hide();
            this.router.navigateByUrl(`parts/assembly`);
          }
        );
  }
}