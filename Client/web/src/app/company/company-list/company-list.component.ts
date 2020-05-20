import { Component, OnInit } from '@angular/core';
import { Company } from '../../models/company.model';
import { CompanyService } from '../company.service';
import { Router } from '@angular/router';
import { UserAction } from '../../models/enum/userAction';
import { DataColumn, DataColumnAction } from '../../models/dataColumn.model';
import { Utils } from '../../common/utils/utils';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { ConfigService } from '../../config/config.service';
import { ClassConstants } from '../../common/constants';
import { ToastrManager } from 'ng6-toastr-notifications';

@Component({
  selector: 'app-company-list',
  templateUrl: './company-list.component.html',
  styleUrls: ['./company-list.component.scss']
})
export class CompanyListComponent implements OnInit {

  companies: Company[] = [];
  columns: DataColumn[] = [];

  constructor(private companyService: CompanyService, private router: Router, private loaderService: httpLoaderService,
              private config: ConfigService, private toastr: ToastrManager) { 

  }

  ngOnInit() {
    this.prepareColumnsList();
    this.loadCompanies();
  }

  prepareColumnsList() {
    this.columns.push( new DataColumn({ headerText: "Name", value: "name", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Address", value: "address", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Phone No", value: "phoneNo", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Email", value: "eMail", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Actions", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Update', actionStyle: ClassConstants.Warning, event: 'editCompany', icon: 'fa fa-edit' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deleteCompany', icon: 'fa fa-trash' })
    ] }) );
  }

  loadCompanies() {
    let that = this;
    this.loaderService.show();
    this.companyService.getAllCompanies()
      .subscribe(
        (companies) => { 
          that.companies = companies;
          that.loaderService.hide();
        },
        (error) => { 
          console.log(error); 
          that.loaderService.hide();
        }
    );
  }

  addCompany() {
    this.router.navigateByUrl(`/companies/detail/${ UserAction.Add }/#`);
  }

  companySelected(id: number) {
    this.router.navigateByUrl(`/companies/detail/${ UserAction.Edit }/${id}`);
  }

  rowSelected(row) {
    this.companySelected(row.id);
  }

  deleteCompany(row) {
    if (confirm('Are you sure you want to remove this company?')) {
      this.companyService.deleteCompany(row.id)
        .subscribe(() => this.toastr.successToastr('Company Removed successfully'));
    }
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'editCompany':
        this.companySelected(data.id);
        break;
      case 'deleteCompany':
        this.deleteCompany(data);
        break;
    }
  }
}
