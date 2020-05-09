import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../../company/company.service';
import { UsermanagementService } from '../usermanagement.service';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { Company } from '../../models/company.model';
import { DataColumn, DataColumnAction } from '../../models/dataColumn.model';
import { ClassConstants } from '../../common/constants';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements OnInit {

  currentlyLoggedInCompany: number = 0;
  privileges: any[] = [];
  users: any[] = [];
  companies: Company[] = [];
  columns: DataColumn[] = []

  user: any = {};

  constructor(private companyService: CompanyService, private userManagementService: UsermanagementService, private httpLoaderService: httpLoaderService,
              private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.populateColumnsForGrid();
    this.loadAllPrivileges();
    this.loadAllCompanies();
    this.getAllUsers();
  }

  populateColumnsForGrid() {
    this.columns.push( new DataColumn({ headerText: "User Name", value: "userName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "First Name", value: "firstName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Last Name", value: "lastName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Email", value: "email" }) );
    this.columns.push( new DataColumn({ headerText: "Super Admin", value: "isSuperAdmin", isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "Actions", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Edit', actionStyle: ClassConstants.Primary, event: 'editUser', icon: 'fa fa-edit' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deleteUser', icon: 'fa fa-trash' })
    ] }) );
  }

  loadAllCompanies() {
    this.companyService.getAllCompanies()
        .subscribe(companies => this.companies = companies);
  }

  getAllUsers() {
    this.userManagementService.getAllUsers()
        .subscribe(users => this.users = users);
  }

  loadAllPrivileges() {
    this.httpLoaderService.show();
    this.userManagementService.getAllPrivileges()
        .subscribe(
          (privileges) => {
            this.privileges = privileges;
            this.httpLoaderService.hide();
          },
          (error) => {
            this.toastr.errorToastr(error.error);
            this.httpLoaderService.hide();
          }
        )
  }
}