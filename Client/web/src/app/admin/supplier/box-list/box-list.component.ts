import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrManager } from 'ng6-toastr-notifications';
import { finalize } from 'rxjs/operators';
import { ClassConstants } from '../../../common/constants';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { CompanyService } from '../../../company/company.service';
import { Box } from '../../../models/box.model';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { UserAction } from '../../../models/enum/userAction';
import { LocationService } from '../location.service';

@Component({
  selector: 'app-box-list',
  templateUrl: './box-list.component.html',
  styleUrls: ['./box-list.component.scss']
})
export class BoxListComponent implements OnInit {

  boxes: Box[] = [];
  columns: DataColumn[] = [];
  currentlyLoggedInCompanyId: number = 0;

  constructor(private companyService: CompanyService, private locationService: LocationService,
    private loaderService: httpLoaderService, private router: Router, private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.getAllBoxes()
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Name", value: "name", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "description", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Location", value: "locationName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, actions: [
      new DataColumnAction({ actionText: 'Update', actionStyle: ClassConstants.Warning, event: 'editBox' }),
      new DataColumnAction({ actionText: 'Print', actionStyle: ClassConstants.Primary, event: 'printBox' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deleteBox' })
    ] }) );
  }

  getAllBoxes() {
    this.loaderService.show();
    this.locationService.getAllBoxes(this.currentlyLoggedInCompanyId)
        .pipe(
          finalize(() => this.loaderService.hide())
        )
        .subscribe(
          (boxes) => {
            this.boxes = boxes;
          }, (error) => {
            console.log(error);
          }
        )
  }

  rowSelected(row) {
    this.router.navigateByUrl(`/suppliers/boxes/${ UserAction.Edit }/${row.id}`);
  }

  addBox() {
    this.router.navigateByUrl(`/suppliers/boxes/${ UserAction.Add }/#`);
  }

  deleteBox(row) {
    if (confirm('Are you sure you want to delete this box?')) {
      this.locationService.deleteBox(row.id)
          .subscribe(
            () => this.router.navigateByUrl('/suppliers/boxes'),
            (error) => this.toastr.errorToastr(error.error)
          );
    }
  }

  printBox(data) {

  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'editLocation':
        this.rowSelected(data);
        break;
      case 'printLocation':
        this.printBox(data);
        break;
      case 'deleteLocation':
        this.deleteBox(data);
        break;
    }
  }

}