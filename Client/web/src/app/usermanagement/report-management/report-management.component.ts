import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../../company/company.service';
import { UsermanagementService } from '../usermanagement.service';
import { DataColumn } from '../../models/dataColumn.model';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { ToastrManager } from 'ng6-toastr-notifications';

@Component({
  selector: 'app-report-management',
  templateUrl: './report-management.component.html',
  styleUrls: ['./report-management.component.scss']
})
export class ReportManagementComponent implements OnInit {

  private privileges: any[] = [];
  private menus: any[] = [];
  private gridColumns: DataColumn[] = [];
  private data: any[] = [];

  constructor(private companyServer: CompanyService, private usermanagementService: UsermanagementService,
              private httpLoaderService: httpLoaderService, private toastr: ToastrManager) { }

  ngOnInit() {
    this.popoulateColumnsForGrid();
    this.getAllPrivilieges();
    this.getAllMenus();
  }

  popoulateColumnsForGrid() {
    this.gridColumns.push( new DataColumn({ headerText: "Column Name", value: "columnName" }) );
    this.gridColumns.push( new DataColumn({ headerText: "Display Name", value: "columnDisplayName", isEditable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Visible", value: "isVisible", isBoolean: true, customStyling: 'center' }) );
  }

  getAllPrivilieges() {
    this.usermanagementService.getAllPrivileges()
        .subscribe(privileges => this.privileges = privileges);
  }

  getAllMenus() {
    this.httpLoaderService.show();
    this.usermanagementService.getAllMenuItems()
        .subscribe(results => {
          results = results.sort((a, b) => (a.menuId > b.menuId) ? 1: -1);
          results.forEach(privilege => {
            if (this.menus.find(m => m.value == privilege.menu) == null)
              this.menus.push({ id: privilege.menuId, value: privilege.menu});
          });
          this.httpLoaderService.hide();
        });
  }

  saveReport() {

  }
}