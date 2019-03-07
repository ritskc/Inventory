import { Component, OnInit } from '@angular/core';
import { Company } from '../../models/company.model';
import { CompanyService } from '../company.service';
import { Router } from '@angular/router';
import { UserAction } from '../../models/enum/userAction';
import { DataColumn } from '../../models/dataColumn.model';
import { Utils } from '../../common/utils/utils';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { ConfigService } from '../../config/config.service';

@Component({
  selector: 'app-company-list',
  templateUrl: './company-list.component.html',
  styleUrls: ['./company-list.component.scss']
})
export class CompanyListComponent implements OnInit {

  companies: Company[] = [];
  columns: DataColumn[] = [];

  constructor(private companyService: CompanyService, private router: Router, private loaderService: httpLoaderService,
              private config: ConfigService) { 

  }

  ngOnInit() {
    this.prepareColumnsList();
    this.loadCompanies();
  }

  prepareColumnsList() {
    // var gridDefinition = this.companyService.getGridPriviledges(localStorage.getItem('username')).subscribe(
    //   (result) => {
    //     if (result && result.reports) {
    //       var priviledges = result.reports.forEach(report => {
    //         if (report.id == this.config.GridConstants.Company) {
    //           report.userReportPriviledges.forEach(column => {
    //             this.columns.push(new DataColumn({ headerText: column.displayName, value: column.columnName, sortable: column.sort, isLink: true }));
    //           });
    //         }
    //       });
    //     }
    //   }
    // )
    this.columns.push( new DataColumn({ headerText: "Name", value: "name", isLink: true, sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Address", value: "address", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Phone No", value: "phoneNo", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Email", value: "eMail", sortable: true }) );
  }

  loadCompanies() {
    let that = this;
    this.loaderService.show();
    this.companyService.getAllCompanies()
      .subscribe(
        (companies) => { 
          setTimeout(function() {
            that.companies = companies;
            that.loaderService.hide();
          }, 2000);
        },
        (error) => { console.log(error); }
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
}
