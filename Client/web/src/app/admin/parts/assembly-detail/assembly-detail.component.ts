import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrManager } from 'ng6-toastr-notifications';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { CompanyService } from '../../../company/company.service';
import { Warehouse } from '../../../models/company.model';
import { DataColumn } from '../../../models/dataColumn.model';
import { UserAction } from '../../../models/enum/userAction';
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
  warehouseId: number = -1;
  mode: UserAction = UserAction.Add;

  columns: DataColumn[] = [];
  warehouses: Warehouse[] = [];

  constructor(private partService: PartsService, private companyService: CompanyService, private toastr: ToastrManager,
              private router: Router, private loadingAnimationService: httpLoaderService, private activateRoute: ActivatedRoute) { }

  ngOnInit() {
    this.mode = this.activateRoute.snapshot.params.mode == 'remove'? UserAction.Delete: UserAction.Add;
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();

    this.columns.push( new DataColumn({ headerText: "Code", value: "subCode", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "subDescription", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Current Inventory", value: "currentInventory", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Required / Assembly", value: "requiredQty", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Total Quantity", value: "quantity", sortable: false, customStyling: 'right' }) );

    this.loadingAnimationService.show();

    this.companyService.getCompany(this.currentlyLoggedInCompany)
        .subscribe((company) => this.warehouses = company.warehouses);

    this.partService.getAnAssembly(this.currentlyLoggedInCompany, this.activateRoute.snapshot.params.id)
        .subscribe(
          (assembly) => this.assembly = assembly,
          (error) => console.log(error.error),
          () => this.loadingAnimationService.hide()
        );
  }

  quantityEntered() {
    if (this.quantity < 1) {
      this.toastr.warningToastr('Please enter a valid quantity');
      return;
    }

    this.assembly.partPartAssignments.forEach(partAssessment => {
      partAssessment.quantity = this.quantity * partAssessment.requiredQty;
    });
  }

  warehouseSelected() {
    this.assembly.partPartAssignments.forEach(partAssignment => {
      partAssignment.currentInventory = partAssignment.warehouseInventories.find(i => i.warehouseId == this.warehouseId).currentInventory;
    });
  }

  save() {
    if (this.quantity < 1) {
      this.toastr.warningToastr('Please enter a valid quantity');
      return;
    }

    if (this.warehouseId == -1) {
      this.toastr.warningToastr('Please select a valid inventory');
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
      warehouseId: this.warehouseId
    };

    this.partService.createAssembly(assembly)
        .subscribe(
          (result) => this.back(),
          (error) => console.log(error.error),
          () => {
            this.loadingAnimationService.hide();
            this.router.navigateByUrl(`parts/assembly`);
          }
        );
  }

  remove() {
    if (this.quantity < 1) {
      this.toastr.warningToastr('Please enter a valid quantity');
      return;
    }

    if (this.warehouseId == -1) {
      this.toastr.warningToastr('Please select a valid inventory');
      return;
    }

    if (confirm(`Are you sure to delete ${ this.quantity } assembly?`)) {
          var body = {
            partId: this.assembly.id,
            qty: this.quantity,
            warehouseId: this.warehouseId
          };
          this.partService.deleteAnAssembly(this.assembly.id, body)
            .subscribe(
              () => this.back(),
              (error) => console.log(error.error),
              () => {}
            );
        }
  }

  back() {
    this.router.navigateByUrl('parts/assembly');
  }
}