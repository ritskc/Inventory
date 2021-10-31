import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrManager } from 'ng6-toastr-notifications';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { CompanyService } from '../../../company/company.service';
import { Warehouse } from '../../../models/company.model';
import { UserAction } from '../../../models/enum/userAction';
import { Location } from '../../../models/location.model';
import { PartsService } from '../../parts/parts.service';
import { LocationService } from '../location.service';

@Component({
  selector: 'app-location-detail',
  templateUrl: './location-detail.component.html',
  styleUrls: ['./location-detail.component.scss']
})
export class LocationDetailComponent implements OnInit {

  warehouseId: number = 0;
  currentlyLoggedInCompanyId: number = 0;
  warehouses: Warehouse[] = [];
  location: Location;

  constructor(private activeRoute: ActivatedRoute, private locationService: LocationService, private companyService: CompanyService,
              private partService: PartsService, private toastr: ToastrManager, private loaderService: httpLoaderService, 
              private router: Router) { 
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.location = new Location();
  }

  ngOnInit() {
    if (this.activeRoute.snapshot.params.action == UserAction.Edit)
      this.getLocation();

    this.companyService.getCompany(this.currentlyLoggedInCompanyId)
        .subscribe(
          (company) => this.warehouses = company.warehouses,
          (error) => console.log(error)
        );
  }

  getLocation() {
    this.locationService.getLocation(this.currentlyLoggedInCompanyId, this.activeRoute.snapshot.params.id)
        .subscribe(
          (location) => {
            this.location = location;
          },
          (error) => console.log(error)
        );
  }

  save() {
    if (!this.location.name || !this.location.description) {
      this.toastr.errorToastr('Please enter location name & description');
      return;
    }

    this.location.companyId = this.currentlyLoggedInCompanyId;

    this.loaderService.show();
    this.locationService.save(this.location)
        .subscribe(
          () => {
            this.toastr.successToastr('Location saved successfully');
            this.loaderService.hide();
            this.router.navigateByUrl('/suppliers/locations');
          }, 
          (error) => console.log(error)
        );
  }
}