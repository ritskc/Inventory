import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../../company/company.service';
import { UsermanagementService } from '../usermanagement.service';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { Company } from '../../models/company.model';
import { DataColumn, DataColumnAction } from '../../models/dataColumn.model';
import { ClassConstants } from '../../common/constants';
import { CustomerService } from '../../admin/customer/customer.service';
import { SupplierService } from '../../admin/supplier/supplier.service';

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
  customerIds: any[] = [];
  supplierIds: any[] = [];
  companies: Company[] = [];
  columns: DataColumn[] = []
  columnsForCustomer: DataColumn[] = [];
  columnsForSupplier: DataColumn[] = [];

  user: any = {
    id: 0,
    priviledgeId: 0,
    userTypeId: 0
  };

  constructor(private companyService: CompanyService, private userManagementService: UsermanagementService, private httpLoaderService: httpLoaderService,
              private toastr: ToastrManager, private customerService: CustomerService, private supplierService: SupplierService) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.populateColumnsForGrid();
    this.loadAllPrivileges();
    this.loadAllCompanies();
    //this.getAllUsers();
  }

  populateColumnsForGrid() {
    this.columns.push( new DataColumn({ headerText: "User Name", value: "userName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "First Name", value: "firstName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Last Name", value: "lastName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Email", value: "email" }) );
    this.columns.push( new DataColumn({ headerText: "Privilege", value: "privilegeName" }) );
    this.columns.push( new DataColumn({ headerText: "Type", value: "type" }) );
    this.columns.push( new DataColumn({ headerText: "Super Admin", value: "isSuperAdmin", isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "Actions", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Edit', actionStyle: ClassConstants.Primary, event: 'editUser', icon: 'fa fa-edit' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deleteUser', icon: 'fa fa-trash' })
    ] }) );

    this.columnsForCustomer.push( new DataColumn({ headerText: "Name", value: "name" }) );
    this.columnsForCustomer.push( new DataColumn({ headerText: "Allowed", value: "allowed", isBoolean: true, customStyling: 'center' }) );
    this.columnsForSupplier.push( new DataColumn({ headerText: "Name", value: "name" }) );
    this.columnsForSupplier.push( new DataColumn({ headerText: "Allowed", value: "allowed", isBoolean: true, customStyling: 'center' }) );
  }

  loadAllCompanies() {
    this.companyService.getAllCompanies()
        .subscribe(companies => this.companies = companies);

    this.customerService.getAllCustomers(this.currentlyLoggedInCompany)
        .subscribe(customers => {
          customers.forEach(customer => {
            this.customerIds.push({id: customer.id, name: customer.name, allowed: false});
          });
        });

    this.supplierService.getAllSuppliers(this.currentlyLoggedInCompany)
        .subscribe(suppliers => {
          suppliers.forEach(supplier => {
            this.supplierIds.push({id: supplier.id, name: supplier.name, allowed: false});
          });
        });
  }

  getAllUsers() {
    this.userManagementService.getAllUsers()
        .subscribe(users => {
          this.users = users;
          this.users.forEach(user => {
            user.privilegeName = this.privileges.find(p => p.id == user.priviledgeId).name;
            switch(user.userTypeId) {
              case 1:
                user.type = "Company";
                break;
              case 2:
                user.type = "Customer";
                break;
              case 3:
                user.type = "Supplier";
                break;
            }
          });
        });
  }

  loadAllPrivileges() {
    this.httpLoaderService.show();
    this.userManagementService.getAllPrivileges()
        .subscribe(
          (privileges) => {
            this.privileges = privileges;
            this.getAllUsers();
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

    if (this.user.userTypeId == 0) {
      this.toastr.errorToastr('Please select the user type');
      return;
    }

    if (this.user.id == 0 && this.users.find(u => u.userName == this.user.userName)) {
      this.toastr.errorToastr('User with this username already exists');
      return;
    }

    if (this.user.id > 0 && this.users.find(u => u.userName == this.user.userName && u.id != this.user.id)) {
      this.toastr.errorToastr('User with this username already exists');
      return;
    }

    this.user.userPriviledge = null;
    this.user.companyIds = [];
    
    if (this.user.userTypeId == 2) {
      this.customerIds.filter(c => c.allowed == true).forEach(selected => this.user.companyIds.push(selected.id));
    } else if (this.user.userTypeId == 3) {
      this.supplierIds.filter(c => c.allowed == true).forEach(selected => this.user.companyIds.push(selected.id));
    } 

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
    switch(data.eventName) {
      case 'editUser':
        this.user = data;
        if (this.user.userTypeId == 2) {
          this.user.companyIds.forEach(ids => {
            this.customerIds.find(c => c.id == ids).allowed = true;
          });
        } else if (this.user.userTypeId == 3) {
          this.user.companyIds.forEach(ids => {
            this.supplierIds.find(c => c.id == ids).allowed = true;
          });
        }
        break;
      case 'deleteUser':
        if (confirm('Are you sure you want to remove this selected user?')) {
          this.httpLoaderService.show();
          this.userManagementService.removeUser(data.id)
              .subscribe(() => {
                this.toastr.successToastr('User removed successfully');
                this.reset();
                this.httpLoaderService.hide();
              })
        }
        break;
    }
    
  }
}