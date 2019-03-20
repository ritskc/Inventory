import { Component, OnInit } from '@angular/core';
import { SupplierService } from '../supplier.service';
import { CompanyService } from '../../../company/company.service';
import { Supplier } from '../../../models/supplier.model';
import { DataColumn } from '../../../models/dataColumn.model';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { UserAction } from '../../../models/enum/userAction';
import { Router } from '@angular/router';

@Component({
  selector: 'app-supplier-list',
  templateUrl: './supplier-list.component.html',
  styleUrls: ['./supplier-list.component.scss']
})
export class SupplierListComponent implements OnInit {

  suppliers: Supplier[] = [];
  columns: DataColumn[] = [];
  currentlyLoggedInCompanyId: number = 0;

  constructor(private supplierService: SupplierService, private companyService: CompanyService,
              private loaderService: httpLoaderService, private router: Router) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.getAllSuppliers();
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Name", value: "name", isLink: true, sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Address", value: "address", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Phone No", value: "phoneNo", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Email", value: "emailID", sortable: true }) );
  }

  getAllSuppliers() {
    this.loaderService.show();
    this.supplierService.getAllSuppliers(this.currentlyLoggedInCompanyId)
      .subscribe(
        (suppliers) => {
          this.suppliers = suppliers;
          this.loaderService.hide();
        },
        (error) => {
          console.log(error);
          this.loaderService.hide();
        }
      )
  }

  rowSelected(row) {
    this.router.navigateByUrl(`/suppliers/detail/${ UserAction.Edit }/${row.id}`);
  }

  addSupplier() {
    this.router.navigateByUrl(`/suppliers/detail/${ UserAction.Add }/#`);
  }
}