import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrManager } from 'ng6-toastr-notifications';
import { forkJoin } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { CompanyService } from '../../../company/company.service';
import { Box } from '../../../models/box.model';
import { UserAction } from '../../../models/enum/userAction';
import { Location } from '../../../models/location.model';
import { LocationService } from '../location.service';

@Component({
  selector: 'app-box-detail',
  templateUrl: './box-detail.component.html',
  styleUrls: ['./box-detail.component.scss']
})
export class BoxDetailComponent implements OnInit {

  locationId: number = 0;
  currentlyLoggedInCompanyId: number = 0;
  locations: Location[] = [];
  box: Box;

  constructor(private activeRoute: ActivatedRoute, private locationService: LocationService, private companyService: CompanyService,
    private toastr: ToastrManager, private loaderService: httpLoaderService, private router: Router) { 
      this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
      this.box = new Box();
    }

  ngOnInit() {
    this.initializePagetData();
  }

  initializePagetData() {
    this.loaderService.show();
    let box$ = this.locationService.getBox(this.currentlyLoggedInCompanyId, this.activeRoute.snapshot.params.id);
    let locations$ = this.locationService.getAllLocations(this.currentlyLoggedInCompanyId);

    forkJoin([box$, locations$])
      .pipe(
        finalize(() => this.loaderService.hide())
      )
      .subscribe(([box, locations]) => {
        this.box = box;
        this.locations = locations;
      });
  }

  save() {
    if (!this.box.name) {
      this.toastr.errorToastr('Please enter box name');
      return;
    }

    this.box.companyId = this.currentlyLoggedInCompanyId;
    this.box.locationId = this.locationId;

    this.loaderService.show();
    this.locationService.saveBox(this.box)
        .pipe(
          finalize(() => this.loaderService.hide())
        )
        .subscribe(
          () => {
            this.toastr.successToastr('Box saved successfully');
            this.router.navigateByUrl('/suppliers/boxes');
          }, 
          (error) => console.log(error)
        );
  }

}
