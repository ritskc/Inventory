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

  companyId: number = 0;
  currentlyLoggedInCompany: number = 0;
  confirmPassword: string = '';

  privileges: any[] = [];
  users: any[] = [];
  companies: Company[] = [];
  columns: DataColumn[] = []

  user: any = {
    id: 0,
    priviledgeId: 0
  };

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

  save() {
    if (!this.user.userName || !this.user.firstName || !this.user.lastName || !this.user.email || !this.user.password || !this.confirmPassword) {
      this.toastr.errorToastr('Please enter all mandatory fields');
      return;
    }

    if (this.user.password !== this.confirmPassword) {
      this.toastr.errorToastr('Both passwords are not same. Please enter the same password.');
      return;
    }

    if (this.user.priviledgeId < 1) {
      this.toastr.errorToastr('Please select the user privilege');
      return;
    }

    if (this.companyId == 0) {
      this.toastr.errorToastr('Please select the company Id');
      return;
    }

    this.user.userPriviledge = null;
    this.user.companyIds = [];
    this.user.companyIds.push(parseInt(this.companyId.toString()));

    this.httpLoaderService.show();
    this.userManagementService.saveUser(this.user)
        .subscribe(
          () => {
            this.toastr.successToastr('User updated succcessfully');
            this.reset();
            this.httpLoaderService.hide();
          },
          (error) => {
            this.toastr.errorToastr(error.error);
            this.httpLoaderService.hide();
          }
        )
  }

  reset() {
    this.user = {};
    this.confirmPassword = '';
    this.getAllUsers();
  }

  actionButtonClickedEvent(data) {
    this.user = data;
  }
}