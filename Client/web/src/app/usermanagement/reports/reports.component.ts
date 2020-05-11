import { Component, OnInit } from '@angular/core';
import { UsermanagementService } from '../usermanagement.service';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { DataColumn, DataColumnAction } from '../../models/dataColumn.model';
import { ClassConstants } from '../../common/constants';
import { ToastrManager } from 'ng6-toastr-notifications';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit {

  userCreatedPrivileges: any[] = [];
  allPrivileges: any[] = [];
  privilgesForMenu: any[] = [];
  menus: any = [];
  columns: DataColumn[] = [];
  columnsForAddedPrivileges: DataColumn[] = [];
  addedPrivileges: any[] = [];
  selectedPrivilege: any = {};

  privilegeId: number = 0;
  menuId: number = 0;
  privilegeName: string = '';
  privilegeDescription: string = '';

  constructor(private userManagementService: UsermanagementService, private httpLoaderService: httpLoaderService, private toastr: ToastrManager) { }

  ngOnInit() {
    this.initializeAllColumns();
    this.getAllPrivileges();
    this.getAllMenus();
  }

  initializeAllColumns() {
    this.columns.push( new DataColumn({ headerText: "Privilege Name", value: "action" }) );
    this.columns.push( new DataColumn({ headerText: "Is Allowed", value: "isPermitted", isBoolean: true, customStyling: 'center' }) );
    this.columnsForAddedPrivileges.push( new DataColumn({ headerText: "Menu Name", value: "menu" }) );
    this.columnsForAddedPrivileges.push( new DataColumn({ headerText: "Privilege Name", value: "action" }) );
    this.columnsForAddedPrivileges.push( new DataColumn({ headerText: "Is Allowed", value: "isPermitted", isBoolean: true, customStyling: 'center' }) );
    this.columnsForAddedPrivileges.push( new DataColumn({ headerText: "Actions", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deletePart', icon: 'fa fa-trash' })
    ] }) );
  }

  getAllMenus() {
    this.httpLoaderService.show();
    this.userManagementService.getAllMenuItems()
        .subscribe(results => {
          this.allPrivileges = results.sort((a, b) => (a.menuId > b.menuId) ? 1: -1);
          this.allPrivileges.forEach(privilege => {
            if (this.menus.find(m => m.value == privilege.menu) == null)
              this.menus.push({ id: privilege.menuId, value: privilege.menu});
          });
          this.httpLoaderService.hide();
        });
  }

  getAllPrivileges() {
    this.userManagementService.getAllPrivileges()
        .subscribe(results => this.userCreatedPrivileges = results);
  }

  privilegeSelected() {
    if (this.privilegeId == 0) {
      this.reset();
    } else {
      this.selectedPrivilege = this.userCreatedPrivileges.find(u => u.id == this.privilegeId);
      this.privilegeName = this.selectedPrivilege.name;
      this.privilegeDescription = this.selectedPrivilege.description;
      this.addedPrivileges = this.selectedPrivilege.userPriviledgeDetails;
    }
  }

  menuSelected() {
    this.privilgesForMenu = this.allPrivileges.filter(p => p.menuId == this.menuId);
    this.privilgesForMenu.forEach(p => p.isPermitted = false);
  }

  addPrivilege() {
    var selectedPrivilegs = this.privilgesForMenu.filter(p => p.isPermitted == true);
    if (selectedPrivilegs) {
      selectedPrivilegs.forEach(selectedPrivilege => {
        if (this.addedPrivileges.findIndex(p => p.id == selectedPrivilege.id) > -1) {
          this.toastr.errorToastr(`This selected privilege ${ selectedPrivilege.menu } --> ${ selectedPrivilege.action } is already added`);
          return;
        } else {
          this.addedPrivileges.push(JSON.parse(JSON.stringify(selectedPrivilege)));
        }
      });
      this.addedPrivileges.sort((a, b) => (a.menuId > b.menuId) ? 1: -1);
    } else {
      this.toastr.errorToastr('Please select at least one privilege to assign');
    }
  }

  actionButtonClicked(data) {
    var index = this.addedPrivileges.findIndex(p => p == data);
    if (index > -1) {
      this.addedPrivileges.splice(index, 1);
    }
  }

  save() {
    if (!this.privilegeName || !this.privilegeDescription) {
      this.toastr.errorToastr('Please enter mandatory fields.');
      return;
    }

    if (this.addedPrivileges && this.addedPrivileges.length == 0) {
      this.toastr.errorToastr('Please select at least one privilege to save');
      return;
    }

    this.httpLoaderService.show();
    this.selectedPrivilege.id = this.privilegeId;
    this.selectedPrivilege.name = this.privilegeName;
    this.selectedPrivilege.description = this.privilegeDescription;
    this.selectedPrivilege.userPriviledgeDetails = this.addedPrivileges;
    this.userManagementService.save(this.selectedPrivilege)
        .subscribe(
          () => {
            this.toastr.successToastr('Privilege saved successfully!!');
            this.getAllPrivileges();
            this.reset();
            this.httpLoaderService.hide();
          },
          (error) => {
            this.toastr.errorToastr(error.error);
            this.httpLoaderService.hide();
          }
        );
  }

  removePrivilege() {
    this.httpLoaderService.show();
    this.userManagementService.removePrivilege(this.privilegeId)
        .subscribe(() => {
          this.toastr.successToastr('Privilege removed successfully');
          this.getAllPrivileges();
          this.reset();
          this.httpLoaderService.hide();
        }, (error) => {
          this.toastr.errorToastr(error.error);
          this.httpLoaderService.hide();
        })
  }

  reset() {
    this.privilegeId = 0;
    this.menuId = 0;
    this.privilgesForMenu = [];
    this.addedPrivileges = [];
    this.privilegeName = '';
    this.privilegeDescription = '';
  }
}