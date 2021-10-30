import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrManager } from 'ng6-toastr-notifications';
import { ClassConstants } from '../../../common/constants';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { CompanyService } from '../../../company/company.service';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { UserAction } from '../../../models/enum/userAction';
import { Location } from '../../../models/location.model';
import { LocationService } from '../location.service';

@Component({
  selector: 'app-location-list',
  templateUrl: './location-list.component.html',
  styleUrls: ['./location-list.component.scss']
})
export class LocationListComponent implements OnInit {

  locations: Location[] = [];
  columns: DataColumn[] = [];
  currentlyLoggedInCompanyId: number = 0;

  constructor(private companyService: CompanyService, private locationService: LocationService,
        private loaderService: httpLoaderService, private router: Router, private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.getAllLocations();
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Name", value: "name", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "description", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, actions: [
      new DataColumnAction({ actionText: 'Update', actionStyle: ClassConstants.Warning, event: 'editLocation' }),
      new DataColumnAction({ actionText: 'Print', actionStyle: ClassConstants.Primary, event: 'printLocation' }),
      new DataColumnAction({ actionText: 'Move Parts', actionStyle: ClassConstants.Primary, event: 'movePartLocation' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deleteLocation' })
    ] }) );
  }

  getAllLocations() {
    this.loaderService.show();
    this.locationService.getAllLocations(this.currentlyLoggedInCompanyId)
        .subscribe(
          (locations) => {
            this.locations = locations;
            this.loaderService.hide();
          }, (error) => {
            console.log(error);
            this.loaderService.hide();
          }
        )
  }

  rowSelected(row) {
    this.router.navigateByUrl(`/suppliers/locations/${ UserAction.Edit }/${row.id}`);
  }

  addLocation() {
    this.router.navigateByUrl(`/suppliers/locations/${ UserAction.Add }/#`);
  }

  deleteLocation(row) {
    if (confirm('Are you sure you want to delete this location?')) {
      this.locationService.delete(row.id)
          .subscribe(
            () => this.router.navigateByUrl('/suppliers/locations'),
            (error) => this.toastr.errorToastr(error.error)
          );
    }
  }

  printLocation(data) {

  }

  movePartLocation(data) {
    
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'editLocation':
        this.rowSelected(data);
        break;
      case 'printLocation':
        this.printLocation(data);
        break;
      case 'movePartLocation':
        this.movePartLocation(data);
        break;
      case 'deleteLocation':
        this.deleteLocation(data);
        break;
    }
  }
}